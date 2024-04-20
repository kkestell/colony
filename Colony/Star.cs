namespace Colony;

class Star
{
    /// <summary>
    /// The type of star, e.g. G-type
    /// </summary>
    public StarType Type { get; init; }
    
    /// <summary>
    /// The mass of the star in solar masses
    /// </summary>
    public double Mass { get; init; }
    
    /// <summary>
    /// The luminosity of the star in solar luminosities
    /// </summary>
    public double Luminosity { get; init; }

    /// <summary>
    /// The radius of the star in solar radii
    /// </summary>
    public double Radius { get; init; }
    
    /// <summary>
    /// The surface temperature of the star in Kelvin
    /// </summary>
    public double Temperature { get; init; }
    
    /// <summary>
    /// The total lifetime of the star in Giga-annums (Ga)
    /// </summary>
    public double Lifetime { get; init; }
    
    /// <summary>
    /// The current age of the star in Giga-annums (Ga)
    /// </summary>
    public double Age { get; init; }

    /// <summary>
    /// The planets orbiting the star
    /// </summary>
    public List<Planet> Planets { get; init; } = [];
    
    /// <summary>
    /// Print the star's properties to the console
    /// </summary>
    public void DebugPrint()
    {
        Console.WriteLine("Star");
        Console.WriteLine($"Type:        {Type}");
        Console.WriteLine($"Mass:        {Mass:F2} M\u2609");
        Console.WriteLine($"Luminosity:  {Luminosity:F4} L\u2609");
        Console.WriteLine($"Radius:      {Radius:F2} R\u2609");
        Console.WriteLine($"Temperature: {Temperature:F0} K");
        Console.WriteLine($"Lifetime:    {Lifetime:F2} Ga");
        Console.WriteLine($"Age:         {Age:F2} Ga");
        Console.WriteLine("Planets");
        foreach (var planet in Planets)
        {
            planet.DebugPrint();
        }
    }
}