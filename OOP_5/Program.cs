using System;
using System.Collections.Generic;

public enum PokemonType
{
    Fire,
    Water,
    Grass
}

public abstract class Pokemon
{
    public string Name { get; }
    public PokemonType Strength { get; }
    public PokemonType Weakness { get; }

    protected Pokemon(string name, PokemonType strength, PokemonType weakness)
    {
        Name = name;
        Strength = strength;
        Weakness = weakness;
    }

    public abstract void BattleCry();
}

public class Squirtle : Pokemon
{
    public Squirtle(string name) : base(name, PokemonType.Water, PokemonType.Grass)
    {
    }

    public override void BattleCry()
    {
        Console.WriteLine(Name + "!!!");
    }
}

public class Bulbasaur : Pokemon
{
    public Bulbasaur(string name) : base(name, PokemonType.Grass, PokemonType.Fire)
    {
    }

    public override void BattleCry()
    {
        Console.WriteLine(Name + "!!!");
    }
}

public class Charmander : Pokemon
{
    public Charmander(string name) : base(name, PokemonType.Fire, PokemonType.Water)
    {
    }

    public override void BattleCry()
    {
        Console.WriteLine(Name + "!!!");
    }
}

public class Pokeball
{
    public bool IsOpen;
    public List<Pokemon> EnclosedPokemons;

    public Pokeball()
    {
        EnclosedPokemons = new List<Pokemon>();
    }

    public void Throw()
    {
        if (!IsOpen && EnclosedPokemons.Count > 0)
        {
            Console.WriteLine("Pokeball is thrown!");
            IsOpen = true;
            ReleasePokemons();
            EnclosedPokemons.Clear(); // Clear the enclosed Pokémon after releasing them
            IsOpen = false; // Set IsOpen back to false after all Pokémon are released
        }
        else
        {
            Console.WriteLine("Pokeball is empty or already open.");
        }
    }

    private void ReleasePokemons()
    {
        foreach (Pokemon pokemon in EnclosedPokemons)
        {
            Console.WriteLine(pokemon.Name + ", I choose you!");
            pokemon.BattleCry();
        }
    }

    public void Return()
    {
        if (IsOpen && EnclosedPokemons.Count > 0)
        {
            foreach (Pokemon pokemon in EnclosedPokemons)
            {
                Console.WriteLine(pokemon.Name + ", come back!");
            }

            EnclosedPokemons.Clear();
            IsOpen = false;
        }
        else
        {
            Console.WriteLine("Pokeball is already closed or empty.");
        }
    }

    public void EnclosePokemon(Pokemon pokemon)
    {
        if (!IsOpen)
        {
            EnclosedPokemons.Add(pokemon);
        }
        else
        {
            Console.WriteLine("Cannot enclose a Pokemon. Pokeball is already open.");
        }
    }
}

public class Trainer
{
    public string Name;
    public List<Pokeball> Belt;

    private const int MaxPokeballs = 6;

    public Trainer(string name)
    {
        Name = name;
        Belt = new List<Pokeball>();
        InitializeBeltWithPokemon();
    }

    private void InitializeBeltWithPokemon()
    {
        int maxPokemonsPerType = MaxPokeballs / PokemonType.GetValues(typeof(PokemonType)).Length;

        foreach (PokemonType type in PokemonType.GetValues(typeof(PokemonType)))
        {
            for (int i = 0; i < maxPokemonsPerType; i++)
            {
                string pokemonName = type.ToString() + (i + 1);
                Pokemon pokemon = CreatePokemonOfType(type, pokemonName);

                if (Belt.Count < MaxPokeballs)
                {
                    Pokeball pokeball = new Pokeball();
                    pokeball.EnclosePokemon(pokemon);
                    Belt.Add(pokeball);
                }
                else
                {
                    throw new Exception("Trainer's belt is full. Cannot add more pokeballs.");
                }
            }
        }
    }

    private Pokemon CreatePokemonOfType(PokemonType type, string name)
    {
        switch (type)
        {
            case PokemonType.Water:
                return new Squirtle(name);
            case PokemonType.Grass:
                return new Bulbasaur(name);
            case PokemonType.Fire:
                return new Charmander(name);
            default:
                throw new ArgumentException("Invalid Pokemon type.");
        }
    }

    public void ThrowPokeballs()
    {
        foreach (Pokeball pokeball in Belt)
        {
            if (pokeball.EnclosedPokemons.Count > 0)
            {
                pokeball.Throw();
            }
        }
    }

    public void ReturnPokeballs()
    {
        foreach (Pokeball pokeball in Belt)
        {
            pokeball.Return();
        }
    }
}

public class Battle
{
    private static int roundCounter;
    private static int battleCounter;

    private static void UpdateScoreboard()
    {
        roundCounter++;
        battleCounter++;
    }

    public static void StartBattle(Trainer trainer1, Trainer trainer2)
    {
        Console.WriteLine("=== Battle Start ===");

        roundCounter = 0;
        battleCounter = 0;

        Random random = new Random();

        while (trainer1.Belt.Count > 0 && trainer2.Belt.Count > 0)
        {
            Console.WriteLine("=== Round " + (roundCounter + 1) + " ===");

            int index1 = random.Next(trainer1.Belt.Count);
            int index2 = random.Next(trainer2.Belt.Count);

            Pokemon pokemon1 = trainer1.Belt[index1].EnclosedPokemons[0];
            Pokemon pokemon2 = trainer2.Belt[index2].EnclosedPokemons[0];

            Console.WriteLine(trainer1.Name + " throws a pokeball with " + pokemon1.Name);
            Console.WriteLine(trainer2.Name + " throws a pokeball with " + pokemon2.Name);

            if (pokemon1.Strength == pokemon2.Weakness)
            {
                Console.WriteLine(pokemon1.Name + " wins!");
                trainer2.Belt[index2].Return();
                trainer2.Belt.RemoveAt(index2);
            }
            else if (pokemon2.Strength == pokemon1.Weakness)
            {
                Console.WriteLine(pokemon2.Name + " wins!");
                trainer1.Belt[index1].Return();
                trainer1.Belt.RemoveAt(index1);
            }
            else
            {
                Console.WriteLine("It's a draw!");
                if (roundCounter > 0)
                {
                    trainer1.Belt[index1].Return();
                    trainer2.Belt[index2].Return();
                }
                else
                {
                    trainer1.Belt[index1].Return();
                    trainer2.Belt[index2].Return();
                    trainer1.Belt.RemoveAt(index1);
                    trainer2.Belt.RemoveAt(index2);
                }
            }

            UpdateScoreboard();
        }

        if (trainer1.Belt.Count > trainer2.Belt.Count)
        {
            Console.WriteLine(trainer1.Name + " wins the battle!");
        }
        else if (trainer2.Belt.Count > trainer1.Belt.Count)
        {
            Console.WriteLine(trainer2.Name + " wins the battle!");
        }
        else
        {
            Console.WriteLine("It's a draw!");
        }

        Console.WriteLine("=== Battle End ===");
    }
}

public class Arena
{
    private static int roundCounter;
    private static int battleCounter;

    public static void Main(string[] args)
    { 
        Console.WriteLine("Naam trainer 1:");
        string nameTrainer1 = Console.ReadLine();
        Trainer trainer1 = new Trainer(nameTrainer1);

        Console.WriteLine("Naam trainer 2:");
        string nameTrainer2 = Console.ReadLine();
        Trainer trainer2 = new Trainer(nameTrainer2);

        StartArenaBattle(trainer1, trainer2);

        Console.WriteLine("Total Battles: " + battleCounter);
        Console.WriteLine("Total Rounds: " + roundCounter);
    }

    private static void UpdateScoreboard()
    {
        roundCounter++;
        battleCounter++;
    }

    public static void StartArenaBattle(Trainer trainer1, Trainer trainer2)
    {
        Console.WriteLine("=== Arena Battle Start ===");

        roundCounter = 0;
        battleCounter = 0;

        Console.WriteLine("Enter the number of battles:");
        int numBattles = int.Parse(Console.ReadLine());

        for (int i = 0; i < numBattles; i++)
        {
            Console.WriteLine("=== Battle " + (battleCounter + 1) + " ===");

            Battle.StartBattle(trainer1, trainer2);

            UpdateScoreboard();

            Console.WriteLine("Press Enter to start the next battle...");
            Console.ReadLine();
        }

        Console.WriteLine("=== Arena Battle End ===");
    }
}