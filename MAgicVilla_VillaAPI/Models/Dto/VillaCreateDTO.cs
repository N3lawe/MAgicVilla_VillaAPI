﻿using System.ComponentModel.DataAnnotations;
namespace MAgicVilla_VillaAPI.Models.Dto;
public record VillaCreateDTO
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }
    public string Details { get; set; }
    [Required]
    public double Rate { get; set; }
    public int Occupancy { get; set; }
    public int Sqft { get; set; }
    public string ImageUrl { get; set; }
    public string Amenity { get; set; }
}