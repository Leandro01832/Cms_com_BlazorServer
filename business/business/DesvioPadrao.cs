using business;
    public class DesvioPadrao : Pagina
    {
        public DesvioPadrao()
        {
            
        }
        public DesvioPadrao(int capitulo)
        {
            Titulo += $" - {capitulo}";
        }
        // Metodo para calcular o desvio padrão de uma lista de números
        // public double CalcularDesvioPadrao(List<double> numeros)
        // {
        //     double media = CalcularMedia(numeros);
        //     double somaQuadrados = 0;

        //     foreach (double numero in numeros)
        //     {
        //         somaQuadrados += Math.Pow(numero - media, 2);
        //     }

        //     return Math.Sqrt(somaQuadrados / numeros.Count);
        // }

        // private double CalcularMedia(List<double> numeros)
        // {
        //     double soma = 0;

        //     foreach (double numero in numeros)
        //     {
        //         soma += numero;
        //     }

        //     return soma / numeros.Count;
        // } 

        public override string ToString()
        {
            return "Desvio Padrao Externo";
        }  
    }    

