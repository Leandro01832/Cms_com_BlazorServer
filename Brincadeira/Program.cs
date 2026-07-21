// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.Xml;
using business.business.brincando;

public class Program
{
    public static void Main()
    {
        var assembly = typeof(Animal).Assembly;
        Random random = new Random();

        var tipos = assembly.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(Animal)) && !t.IsAbstract).ToList();
        var lista = new List<object>();

        foreach (var item in tipos)
            lista.Add(Activator.CreateInstance(item)!);

            Comportamento[] arr = new Comportamento[16];

        arr[0]  = new Comportamento { Acao = "Coacha. Ccoach, ccoach, ccoach!!!!!" };
        arr[1]  = new Comportamento { Acao = "pia. piu piu piu... piu piu piu!!!!!" };
        arr[2]  = new Comportamento { Acao = "canta. Uuuuooooooooonnnnn (canto de baleia)!!!!!" };
        arr[3]  = new Comportamento { Acao = "late. Au au au... au au au!!!!!" };
        arr[4]  = new Comportamento { Acao = "Mia. Miaaaauuuuu!!!!!" };
        arr[5]  = new Comportamento { Acao = "Zuni. Zzzzz zzzzz zzzzzz!!!!!" };
        arr[6]  = new Comportamento { Acao = "Chia. Ssssssssssss... sssssssss!!!!!" };
        arr[7]  = new Comportamento { Acao = "pula." };
        arr[8]  = new Comportamento { Acao = "Caça." };
        arr[9] = new Comportamento  { Acao = "voa." };
        arr[10] = new Comportamento { Acao = "faz fotossíntese." };
        arr[11] = new Comportamento { Acao = "nadam" };
        arr[12] = new Comportamento { Acao = "sobe em árvore" };
        arr[13] = new Comportamento { Acao = "Se alimentam" };
        arr[14] = new Comportamento { Acao = "Se auto alimentam" };
        arr[15] = new Comportamento { Acao = "amamentar seus filhotes" };

        foreach(var item in lista)
        {
            ((Animal)item).Comportamentos = new List<Comportamento>();
            if(item is Baleia)
                ((Animal)item).Comportamentos.Add(arr[2]);
            
            if(item is Reptil)
                ((Animal)item).Comportamentos.Add(arr[6]);
            if(item is Aves)
            {
                ((Animal)item).Comportamentos.Add(arr[1]);
                ((Animal)item).Comportamentos.Add(arr[9]);                
            }
            if(item is Anfibio)
            {
                ((Animal)item).Comportamentos.Add(arr[0]);
                ((Animal)item).Comportamentos.Add(arr[7]);                
            }
             if(item is Canidae)
                ((Animal)item).Comportamentos.Add(arr[3]);
                if(item is Felidae)
                ((Animal)item).Comportamentos.Add(arr[4]);
                if(item is Inseto)
                ((Animal)item).Comportamentos.Add(arr[5]);
             if(item is Animal)
                ((Animal)item).Comportamentos.Add(arr[13]);
             if(item is Mamifero)
                ((Mamifero)item).Comportamentos.Add(arr[15]);
            
        }


        var animais = lista
        .Select(animal => ((Animal)animal).ExecutarAcao())
        .ToList();
        foreach (var item in animais)
        {
            
            Console.WriteLine(item + "\n");
        }

        var a = new Canidae();
        a.Comportamentos = new List<Comportamento>();
        a.Comportamentos.Add(arr[random.Next(0, arr.Length)]);
        var b = new Felidae();
        b.Comportamentos = new List<Comportamento>();
        b.Comportamentos.Add(arr[random.Next(0, arr.Length)]);
        var c = new Inseto();
        c.Comportamentos = new List<Comportamento>();
        c.Comportamentos.Add(arr[random.Next(0, arr.Length)]);
        var d = new Aves();
        d.Comportamentos = new List<Comportamento>();
        d.Comportamentos.Add(arr[random.Next(0, arr.Length)]);
        var e = new Anfibio();
        e.Comportamentos = new List<Comportamento>();
        e.Comportamentos.Add(arr[random.Next(0, arr.Length)]);
        var f = new Baleia();
        f.Comportamentos = new List<Comportamento>();
        f.Comportamentos.Add(arr[random.Next(0, arr.Length)]);
        var g = new Reptil();
        g.Comportamentos = new List<Comportamento>();
        g.Comportamentos.Add(arr[random.Next(0, arr.Length)]);

        Console.WriteLine(a.ExecutarAcao() + "\n");
        Console.WriteLine(b.ExecutarAcao() + "\n");
        Console.WriteLine(c.ExecutarAcao() + "\n");
        Console.WriteLine(d.ExecutarAcao() + "\n");
        Console.WriteLine(e.ExecutarAcao() + "\n");
        Console.WriteLine(f.ExecutarAcao() + "\n");
        Console.WriteLine(g.ExecutarAcao() + "\n");
    }
}
