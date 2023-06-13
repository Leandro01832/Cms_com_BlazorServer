using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace business
{
    public abstract class BaseModel 
    {
        [Key]
        public Int64 Id { get; set; }
        
        [NotMapped]
        public string Tipo
        {
            get { return this.GetType().Name; }
        }

        [NotMapped]
        public static bool entity = false;


        public static List<Type> listTypesSon(Type tipo)
        {
            var listaTypes = tipo.Assembly.GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(tipo)).ToList();

            return listaTypes;
        }

        
    }
}
