using System.ComponentModel.DataAnnotations;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Models;

public class Ascent {
    public int Id {get; set;}
    public User User {get; set;}
    private Ascent(){}
    public Ascent(User user) {
        User = user;
    }
}
