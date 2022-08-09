const img = document.getElementById('img');
const svg = document.getElementById('svg');
const SVG_URL = "http://www.w3.org/2000/svg";
let dragObj = null, dragObjParent = null, relatedObjs = null, draggingPoint = null, startCursor = { x: 0, y: 0 };
let onMousemoveFuncs = { 'ellipse': [movingEllipse, reshapingEllipse], 'polygon': [movingPolygon, reshapingPolygon] };
svg.onmouseup = drop;
svg.onmouseleave = drop;


function fetchCurrentWall() {
    fetch('../../v1/walls/latest')
        .then(response => {
            if (response.ok) {
                console.log(response)
                return response.json()
            }
        }).then(data => {
            img.onload = () => DeserializeSVG(data.ellipseHolds, data.polygonHolds);
            img.src = '../walls/' + data.uri;
        })
}
function imgInput() {
    for (let svg_child of svg.children) {
        svg_child.parentElement.removeChild(svg_child);
    }
    const file = document.getElementById("upload-img").files[0]
    img.src = URL.createObjectURL(file)

}

function uploadSetup() {
    console.log("#WFasdf")
    const file = document.getElementById("upload-setup").files[0]
    let fr = new FileReader();
    fr.onload = (result) => {
        let data = JSON.parse(fr.result);
        DeserializeSVG(data.EllipseHolds, data.PolygonHolds);
    }
    fr.readAsText(file);
    console.log(file)
}

function saveCurrent() {
    let [ellipseHolds, polygonHolds] = SerializeSVG();
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(JSON.stringify({ EllipseHolds: ellipseHolds, PolygonHolds: polygonHolds })));
    element.setAttribute('download', "wall.json");

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

function uploadFile() {
    let username = document.getElementById("username").value;
    let password = document.getElementById("password").value;
    if (!username || !password) {
        alert("ingen brukernavn ellr passord");
    }

    let name = prompt("Wall name", "default")

    let [ellipseHolds, polygonHolds] = SerializeSVG();
    const formData = new FormData();
    const file = document.getElementById("upload-img").files[0]
    const fileField = document.querySelector('input[type="file"]');
    formData.append("wallImage", file, "wallImage");
    formData.append("JsonData", JSON.stringify({
        Name: name,
        EllipseHolds: ellipseHolds,
        PolygonHolds: polygonHolds,
    }))
    fetch('../../v1/identity/login', {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            username: username,
            password: password,
        })
    })
        .then(response => {
            if (!response.ok) {
                throw Error("LOGIN FAILED")
            }
            return response.text();
        })
        .then(() =>
            fetch("../../v1/walls/upload", {
                method: "POST",
                body: formData,
            })
        )
        .then(response => {
            if (!response.ok) {
                throw Error("UPLOAD FAILED")
            }
            alert("uploaded")
        })
        .catch(error => { alert(error.message) })
}

function SerializeSVG() {
    let ellipseHolds = [];
    let polygonHolds = [];
    let i = 0;
    for (let hold of svg.querySelectorAll('.svg-child')) {
        if (hold.tagName == "ellipse") {
            let cx = hold.getAttributeNS(null, 'cx');
            let cy = hold.getAttributeNS(null, 'cy');
            let rx = hold.getAttributeNS(null, 'rx');
            let ry = hold.getAttributeNS(null, 'ry');
            let rotation = hold.getAttributeNS(null, 'transform');
            let rotationStart = rotation.indexOf('rotate(') + 7
            rotation = rotation.slice(rotationStart, rotationStart + rotation.slice(rotationStart).indexOf(','))
            ellipseHolds.push({
                index: i,
                cx: Number((cx / img.clientWidth)),
                cy: Number((cy / img.clientHeight)),
                rx: Number((rx / img.clientWidth)),
                ry: Number((ry / img.clientHeight)),
                rotation: Number(rotation)
            })
            i++;
        }
        else if (hold.tagName == "polygon") {
            let points = hold.getAttributeNS(null, "points");
            polygonHolds.push(
                {
                    index: i,
                    Points: points.split(" ").filter(i => i.split(",").length == 2).map(p => {
                        let [x, y] = p.split(',');
                        return { x: Number((x / img.clientWidth)), y: Number((y / img.clientHeight)) }
                    })
                }
            )
            i++;
        }
    }
    return [ellipseHolds, polygonHolds]
}

function DeserializeSVG(ellipseHolds, polygonHolds) {
    for (var ellipseHold of ellipseHolds) {
        AddEllipse(
            ellipseHold.cx * img.clientWidth,
            ellipseHold.cy * img.clientHeight,
            ellipseHold.rx * img.clientWidth,
            ellipseHold.ry * img.clientHeight,
            ellipseHold.rotation,
        );
    }
    for (var polygonHold of polygonHolds) {
        AddPolygon(polygonHold.points.length, polygonHold.points.map(p => [p.x * img.clientWidth, p.y * img.clientHeight]));
    }
}




function deleteAllSelected() {
    for (let selectedChild of svg.querySelectorAll('.selected')) {
        svg.removeChild(selectedChild);
    }
}

function unselectAll() {
    for (let selectedChild of svg.querySelectorAll('.selected')) {
        selectedChild.classList.remove('selected');
        selectedChild.classList.add('unselected');
    }
}

function select(obj, e) {
    e.stopPropagation()
    for (let relatedObj of svg.querySelectorAll(`[id^=${obj.id}_]`)) {
        relatedObj.classList.add('selected');
        relatedObj.classList.remove('unselected');
    }
}


function rel(id) {
    return id ? document.getElementById(id) : null
}

function relParent(id) {
    return id ? document.getElementById(id.slice(0, id.indexOf('_', 4))) : null
}

function makeObjectToDrag(obj, e) {
    dragObj = rel(obj.id);
    dragObjParent = relParent(obj.id);
    relatedObjs = svg.querySelectorAll(`[id^=${dragObjParent.id}_]:not([id=${dragObjParent.id}], [id=${dragObj.id}])`);
    startCursor.x = dragObj.getAttributeNS(null, 'cx'), startCursor.y = dragObj.getAttributeNS(null, 'cy');
    svg.onmousemove = onMousemoveFuncs[dragObjParent.nodeName][obj.id.includes('drag') ? 0 : 1];
}


function drop() { dragObj = null; dragObjParent = null; relatedObjs = null, draggingPoint = null; };

function movingEllipse(e) {
    if (dragObj && dragObjParent) {
        dragObj.setAttributeNS(null, 'cx', e.layerX)
        dragObj.setAttributeNS(null, 'cy', e.layerY)
        dragObjParent.setAttributeNS(null, 'cx', e.layerX)
        dragObjParent.setAttributeNS(null, 'cy', e.layerY)
        for (let relatedObj of relatedObjs) {
            let x = parseInt(relatedObj.getAttributeNS(null, 'cx'));
            let y = parseInt(relatedObj.getAttributeNS(null, 'cy'));
            relatedObj.setAttributeNS(null, 'cx', x + (e.layerX - startCursor.x))
            relatedObj.setAttributeNS(null, 'cy', y + (e.layerY - startCursor.y))
        }
        startCursor.x = e.layerX, startCursor.y = e.layerY;
    }
}

function reshapingEllipse(e) {
    if (dragObj && dragObjParent) {
        if (dragObj.id[dragObj.id.length - 1] == 'x') {
            let x = parseInt(dragObjParent.getAttributeNS(null, 'cx'));
            if (e.ctrlKey) {
                let y = parseInt(dragObjParent.getAttributeNS(null, 'cy'));
            } else {
                dragObj.setAttributeNS(null, 'cx', e.layerX)
                let x = parseInt(dragObjParent.getAttributeNS(null, 'cx'));
                dragObjParent.setAttributeNS(null, 'rx', Math.abs(e.layerX - x))
            }
        } else {
            let y = parseInt(dragObjParent.getAttributeNS(null, 'cy'));
            if (e.ctrlKey) {
                let x = parseInt(dragObjParent.getAttributeNS(null, 'cx'));
                dragObjParent.setAttributeNS(null, 'transform', `rotate(${e.layerY - startCursor.y}, ${x}, ${y})`)
            } else {
                dragObj.setAttributeNS(null, 'cy', e.layerY)
                dragObjParent.setAttributeNS(null, 'ry', Math.abs(e.layerY - y))
            }
        }
    }
}

function AddEllipse(cx = null, cy = null, rx = null, ry = null, rotation = null) {
    const IMG_WIDTH = img.clientWidth / 2;
    const IMG_HEIGHT = img.clientHeight / 2;
    const ELLIPSE_DEFAULT = {
        cx: cx ? cx : IMG_WIDTH,
        cy: cy ? cy : IMG_HEIGHT,
        rx: rx ? rx : 25,
        ry: ry ? ry : 25,
        transform: rotation ? `rotate(${rotation},${cx ? cx : IMG_WIDTH}, ${cy ? cy : IMG_WIDTH})` : "",
        onclick: 'select(this,event)'
    }
    const DRAG_DEFAULT = {
        cx: cx ? cx : IMG_WIDTH,
        cy: cy ? cy : IMG_HEIGHT,
        r: 5,
        onmousedown: 'makeObjectToDrag(this, event)',
    }
    const POINT_X = {
        cx: cx ? cx + rx : IMG_WIDTH + 25,
        cy: cy ? cy : IMG_HEIGHT,
        r: 5,
        onmousedown: 'makeObjectToDrag(this, event)',
    }
    const POINT_Y = {
        cx: cx ? cx : IMG_WIDTH,
        cy: cy ? cy - ry : IMG_HEIGHT - 25,
        r: 5,
        onmousedown: 'makeObjectToDrag(this, event)',
    }

    let svgCount = svg.querySelectorAll('.svg-child').length;

    let svg_child = svg.appendChild(document.createElementNS(SVG_URL, 'ellipse'));
    svg_child.classList.add('svg-child', 'selected');
    svg_child.id = 'svg_' + svgCount;
    for (let atr in ELLIPSE_DEFAULT) {
        svg_child.setAttributeNS(null, atr, ELLIPSE_DEFAULT[atr]);
    }

    let drag = svg.appendChild(document.createElementNS(SVG_URL, 'circle'));
    drag.id = 'svg_' + svgCount + '_drag'
    drag.classList.add('drag', 'selected');
    for (let atr in DRAG_DEFAULT) {
        drag.setAttributeNS(null, atr, DRAG_DEFAULT[atr]);
    }

    let pointX = svg.appendChild(document.createElementNS(SVG_URL, 'circle'));
    pointX.id = 'svg_' + svgCount + '_point_x';
    pointX.classList.add('point', 'selected')
    for (let atr in POINT_X) {
        pointX.setAttributeNS(null, atr, POINT_X[atr]);
    }

    let pointY = svg.appendChild(document.createElementNS(SVG_URL, 'circle'));
    pointY.id = 'svg_' + svgCount + '_point_y';
    pointY.classList.add('point', 'selected')
    for (let atr in POINT_Y) {
        pointY.setAttributeNS(null, atr, POINT_Y[atr]);
    }
}

function movingPolygon(e) {
    if (dragObj && dragObjParent) {
        dragObj.setAttributeNS(null, 'cx', e.layerX)
        dragObj.setAttributeNS(null, 'cy', e.layerY)
        let points = [];
        for (let relatedObj of relatedObjs) {
            let x = parseInt(relatedObj.getAttributeNS(null, 'cx'));
            let y = parseInt(relatedObj.getAttributeNS(null, 'cy'));
            points.push([x, y]);
            relatedObj.setAttributeNS(null, 'cx', x + (e.layerX - startCursor.x))
            relatedObj.setAttributeNS(null, 'cy', y + (e.layerY - startCursor.y))
        }
        dragObjParent.setAttributeNS(null, 'points', points.map((p) => p.join(',')).join(' '));
        startCursor.x = e.layerX, startCursor.y = e.layerY;
    }
}

function reshapingPolygon(e) {
    if (dragObj && dragObjParent) {
        dragObj.setAttributeNS(null, 'cx', e.layerX)
        dragObj.setAttributeNS(null, 'cy', e.layerY)
        let points = dragObjParent.getAttributeNS(null, 'points').split(' ');
        let pointIndex = dragObj.id.slice(dragObj.id.lastIndexOf('_') + 1);
        points[pointIndex] = `${e.layerX},${e.layerY}`;
        dragObjParent.setAttributeNS(null, 'points', points.join(' '));
    }
}

function AddPolygon(n, points = null) {
    const IMG_WIDTH = img.clientWidth / 2;
    const IMG_HEIGHT = img.clientHeight / 2;
    let svgCount = svg.querySelectorAll('.svg-child').length;
    let ListOfPoints;
    if (points) {
        ListOfPoints = points;
    } else {
        if (n == 3) {
            ListOfPoints = [[IMG_WIDTH - 25, IMG_HEIGHT + 25], [IMG_WIDTH, IMG_HEIGHT - 25], [IMG_WIDTH + 25, IMG_HEIGHT + 25]];
        }
        else if (n == 4) {
            ListOfPoints = [[IMG_WIDTH - 25, IMG_HEIGHT - 25], [IMG_WIDTH + 25, IMG_HEIGHT - 25], [IMG_WIDTH + 25, IMG_HEIGHT + 25], [IMG_WIDTH - 25, IMG_HEIGHT + 25]]
        } else {
            ListOfPoints = [[IMG_WIDTH - 50, IMG_HEIGHT], [IMG_WIDTH - 25, IMG_HEIGHT - 25], [IMG_WIDTH, IMG_HEIGHT - 50], [IMG_WIDTH + 25, IMG_HEIGHT - 25], [IMG_WIDTH + 50, IMG_HEIGHT], [IMG_WIDTH + 25, IMG_HEIGHT + 25], [IMG_WIDTH, IMG_HEIGHT + 50], [IMG_WIDTH - 25, IMG_HEIGHT + 25]];
        }
    }

    let polygon_list_string = "";
    let svg_child = svg.appendChild(document.createElementNS(SVG_URL, 'polygon'));
    svg_child.classList.add('svg-child', 'selected');
    svg_child.id = 'svg_' + svgCount;
    svg_child.setAttributeNS(null, 'onclick', 'select(this, event)')


    let drag = svg.appendChild(document.createElementNS(SVG_URL, 'circle'));
    drag.id = 'svg_' + svgCount + '_drag';
    drag.classList.add('drag', 'selected');
    drag.setAttributeNS(null, 'cx', IMG_WIDTH);
    drag.setAttributeNS(null, 'cy', IMG_HEIGHT);
    drag.setAttributeNS(null, 'r', 5);
    drag.setAttributeNS(null, 'onmousedown', 'makeObjectToDrag(this, event)');
    var i = 0;
    for (let points of ListOfPoints) {
        let point = svg.appendChild(document.createElementNS(SVG_URL, 'circle'));
        point.id = 'svg_' + svgCount + '_point_' + i;
        point.classList.add('point', 'selected');
        let [x, y] = points;
        point.setAttributeNS(null, 'cx', x);
        point.setAttributeNS(null, 'cy', y);
        point.setAttributeNS(null, 'r', 5);
        point.setAttributeNS(null, 'onmousedown', 'makeObjectToDrag(this, event)')
        polygon_list_string += points.join(',') + " ";
        i++;
    }
    svg_child.setAttributeNS(null, 'points', polygon_list_string);
}
