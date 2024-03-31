using System;
using System.Collections.Generic;
using LiteDB;
using UnityEngine;
namespace ScriptsBox.DataBase.LiteDB
{
    public class LiteDB : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, "DataBase/MyDB.db");
            using (var db = new LiteDatabase(path))
            {
                var col = db.GetCollection<Customer>("customers");
                col.DeleteAll();
                Weapon sword = new Weapon() { Name = "刀", Damage = 1 };
                Weapon jian = new Weapon() { Name = "剑", Damage = 2 };
                Dish apple = new Dish()
                {
                    Food = "Apple",
                    Weapons = new List<Weapon>()
                    {
                        sword,
                        jian,
                    }
                };
                Dish orange = new Dish()
                {
                    Food = "Orange",
                    Weapons = new List<Weapon>()
                    {
                        jian
                    }
                };
                var customer = new Customer()
                {
                    Name = 1,
                    IsActive = true,
                    Templates = new List<ETemplate>(2) { ETemplate.A, ETemplate.C },
                    Dishes = new List<Dish>(2)
                    {
                        apple,orange
                    }
                };
                for (int i = 0; i < 1000; i++)
                {
                    customer.Name = i;
                    col.Insert(customer); 
                }
                
                /*col.Update(customer);*/
                    
            }

        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

    public enum ETemplate
    {
        A,B,C
    }

    [Serializable]
    public class Weapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }
    }
    [Serializable]
    public class Dish
    {
        public string Food { get; set; }
        public List<Weapon> Weapons { get; set; }
    }

    [Serializable]
    public class Customer
    {
        public int Name { get; set; }
        public bool IsActive { get; set; }
        
        public List<Dish> Dishes { get; set; }
        public List<ETemplate> Templates { get; set; }
    }
}
