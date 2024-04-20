namespace Colony;

internal class Genesis(int size)
{
    const double StefanBoltzmannConstant = 5.67e-8; // W/m^2/K^4

    public static Star CreateStar()
    {
        var type = (StarType)Rng.Next(Enum.GetNames(typeof(StarType)).Length);
        
        var massRanges = new Dictionary<StarType, (double, double)>
        {
            { StarType.O, (16, 150) },    // O-type stars: min 16 M☉, max can exceed 150 M☉
            { StarType.B, (2.1, 16) },    // B-type stars: min 2.1 M☉, max 16 M☉
            { StarType.A, (1.4, 2.1) },   // A-type stars: min 1.4 M☉, max 2.1 M☉
            { StarType.F, (1.04, 1.4) },  // F-type stars: min 1.04 M☉, max 1.4 M☉
            { StarType.G, (0.8, 1.04) },  // G-type stars: min 0.8 M☉, max 1.04 M☉
            { StarType.K, (0.45, 0.8) },  // K-type stars: min 0.45 M☉, max 0.8 M☉
            { StarType.M, (0.08, 0.45) }  // M-type stars: min 0.08 M☉, max 0.45 M☉
        };
        
        var (minMass, maxMass) = massRanges[type];
        var mass = Rng.NextDouble() * (maxMass - minMass) + minMass;

        // Luminosity = M^4
        var luminosity = Math.Pow(mass, 4);

        // Radius = M^0.8
        var radius = Math.Pow(mass, 0.8);

        // Temperature from Stefan-Boltzmann Law: L = 4πR^2σT^4
        var luminosityInWatts = luminosity * 3.828e26; // Convert solar luminosities to watts
        var radiusInMeters = radius * 6.957e8; // Convert solar radii to meters
        var temperature = Math.Pow(luminosityInWatts / (4 * Math.PI * Math.Pow(radiusInMeters, 2) * StefanBoltzmannConstant), 0.25);

        // Lifetime in Ga = 10 / M^2.5
        var lifetime = 10 / Math.Pow(mass, 2.5);

        // Generate a random age that is a fraction of the star's total expected lifetime
        // Assuming that the star can be up to 90% through its main sequence phase
        var factor = Math.Pow(Rng.NextDouble(), 2); // Using the square to skew towards younger ages
        var age = Math.Min(factor * lifetime, 13.8);

        var star = new Star
        {
            Type = type,
            Mass = mass,
            Luminosity = luminosity,
            Radius = radius,
            Temperature = temperature,
            Lifetime = lifetime,
            Age = age
        };
        
        var planet = CreatePlanet(star);
        star.Planets.Add(planet);

        return star;
    }
    
    static Planet CreatePlanet(Star star)
    {
        var type = (PlanetType)Rng.Next(Enum.GetNames(typeof(PlanetType)).Length);

        // Orbital distance
        var innerEdge = 0.95 * Math.Sqrt(star.Luminosity);
        var outerEdge = 1.37 * Math.Sqrt(star.Luminosity);

        double orbitalDistance;
        switch (type)
        {
            case PlanetType.DesertPlanet:
                // DesertPlanets are within 10% of the inner edge
                orbitalDistance = innerEdge + 0.1 * (outerEdge - innerEdge) * Rng.NextDouble();
                break;
            case PlanetType.OceanPlanet:
                // OceanPlanets are in the middle 1/3 of the habitable zone
                var oceanInner = innerEdge + (outerEdge - innerEdge) / 3;
                var oceanOuter = innerEdge + 2 * (outerEdge - innerEdge) / 3;
                orbitalDistance = Rng.NextDouble() * (oceanOuter - oceanInner) + oceanInner;
                break;
            case PlanetType.EarthLike:
                // EarthLike planets can span the entire habitable zone but favor the middle
                var earthLikeInner = innerEdge + 0.25 * (outerEdge - innerEdge);
                var earthLikeOuter = innerEdge + 0.75 * (outerEdge - innerEdge);
                orbitalDistance = Rng.NextDouble() * (earthLikeOuter - earthLikeInner) + earthLikeInner;
                break;
            // Add other planet types as needed with specific rules
            case PlanetType.RockySuperEarth:
            case PlanetType.IronPlanet:
            case PlanetType.CarbonPlanet:
            default:
                // For other types, use a random position within the habitable zone
                orbitalDistance = Rng.NextDouble() * (outerEdge - innerEdge) + innerEdge;
                break;
        }

        // Mass
        var massRanges = new Dictionary<PlanetType, (double MinMass, double MaxMass)>
        {
            { PlanetType.EarthLike, (0.8, 1.2) },
            { PlanetType.RockySuperEarth, (1.5, 10) },
            { PlanetType.OceanPlanet, (0.9, 5) },
            { PlanetType.DesertPlanet, (0.5, 1.5) },
            { PlanetType.IronPlanet, (1, 3) },
            { PlanetType.CarbonPlanet, (1, 3) },
        };
        var (minMass, maxMass) = massRanges[type];
        var mass = Rng.NextDouble() * (maxMass - minMass) + minMass;

        // Radius
        var radius = Math.Pow(mass, 0.3);
        
        // Density
        // Convert mass from Earth masses to kilograms (1 M⊕ ≈ 5.972 × 10^24 kg)
        // Convert radius from Earth radii to meters (1 R⊕ ≈ 6.371 × 10^6 m)
        var massInKg = mass * 5.972e24;
        var radiusInMeters = radius * 6.371e6;
        // Calculate density in kg/m^3, then convert to g/cm^3 (1 kg/m^3 = 0.001 g/cm^3)
        var volume = (4.0 / 3.0) * Math.PI * Math.Pow(radiusInMeters, 3);
        var densityInKgPerM3 = massInKg / volume;
        var density = densityInKgPerM3 * 0.001;

        // Gravity
        // Calculate surface gravity in m/s^2
        const double gravitationalConstant = 6.674e-11;
        var gravity = gravitationalConstant * massInKg / Math.Pow(radiusInMeters, 2);
        
        // Age
        var starAge = star.Age;
        // Planetary formation time can be considered, for simplicity, a small fraction of the star's age
        const double formationTimeFraction = 0.05; // For example, 5% of the star's age
        var minPlanetAge = starAge * formationTimeFraction;
        // Generate a random age for the planet between the minimum possible age and the star's age
        var age = Rng.NextDouble() * (starAge - minPlanetAge) + minPlanetAge;
        
        var planet = new Planet
        {
            Type = type,
            Mass = mass,
            Radius = radius,
            Density = density,
            OrbitalDistance = orbitalDistance,
            Gravity = gravity,
            Age = age
        };
        
        return planet;
    }
    
    public Map CreateMap()
    {
        var map = new Map(size);
        
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                map.Tiles[x, y].TileNum = (Glyph)Rng.Next(256);
                map.Tiles[x, y].Foreground = Color.White;
                map.Tiles[x, y].Background = Color.Black;
            }
        }

        return map;
    }
}
