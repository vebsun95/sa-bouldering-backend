using System.ComponentModel.DataAnnotations;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Models;

public class Ascent {
    public int Id {get; set;}
    public Boulder Route {get; set;}
    public ApplicationUser User {get; set;}
    private Ascent(){}
    public Ascent(Boulder route, ApplicationUser user) {
        Route = route;
        User = user;
    }
}
