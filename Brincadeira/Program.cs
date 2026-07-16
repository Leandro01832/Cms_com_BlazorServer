// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.Xml;
using business.business.brincando;

public class Program
{
    public static void Main()
    {        
        var assembly = typeof(Animal).Assembly;    

        var tipos = assembly.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(Animal)) && !t.IsAbstract).ToList();
        var listaAnimaisOriginal = new List<Animal>();

        foreach(var item in tipos)
        listaAnimaisOriginal.Add((Animal)Activator.CreateInstance(item)!);
        var barulhosDosAnimais = listaAnimaisOriginal
        .Select(animal => animal.FazerBarulho(animal.GetType()))
        .ToList();        
        foreach(var item in barulhosDosAnimais)
        Console.WriteLine(item + "\n");

        var a = new Canidae();
        var b = new Felidae();
        var c = new Inseto();

        Console.WriteLine(a.FazerBarulho(b.GetType()) + "\n");
        Console.WriteLine(b.FazerBarulho(a.GetType()) + "\n");
        Console.WriteLine(c.FazerBarulho(c.GetType()) + "\n");        
    }
}
