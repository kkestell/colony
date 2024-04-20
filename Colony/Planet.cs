namespace Colony;

class Planet
{
    /// <summary>
    /// The type of planet, e.g. terrestrial
    /// </summary>
    public PlanetType Type { get; init;  }
    
    /// <summary>
    /// The mass of the planet in Earth masses
    /// </summary>
    public double Mass { get; init;}

    /// <summary>
    /// The radius of the planet in Earth radii
    /// </summary>
    public double Radius { get; init; }

    /// <summary>
    /// The density of the planet in g/cm^3
    /// </summary>
    public double Density { get; init; }

    /// <summary>
    /// The orbital distance of the planet from its host star in Astronomical Units (AU)
    /// </summary>
    public double OrbitalDistance { get; init; }
    
    /// <summary>
    /// The orbital period of the planet in Earth years
    /// </summary>
    public double OrbitalPeriod => Math.Sqrt(Math.Pow(OrbitalDistance, 3)); // Kepler's Third Law
    
    /// <summary>
    /// The gravity of the planet in m/s^2
    /// </summary>
    public double Gravity { get; init; }
    
    /// <summary>
    /// The age of the planet in Giga-annums (Ga)
    /// </summary>
    public double Age { get; init; }
    
    public void DebugPrint()
    {
        Console.WriteLine($"Type:        {Type}");
        Console.WriteLine($"Mass:        {Mass:F2} M\u2295");
        Console.WriteLine($"Radius:      {Radius:F2} R\u2295");
        Console.WriteLine($"Density:     {Density:F2} g/cm\u00B3");
        Console.WriteLine($"Orbit:       {OrbitalDistance:F2} AU");
        Console.WriteLine($"Period:      {OrbitalPeriod:F2} yr");
        Console.WriteLine($"Gravity:     {Gravity:F2} m/s\u00B2");
        Console.WriteLine($"Age:         {Age:F2} Ga");
    }
}