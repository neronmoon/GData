using System;
using GData.Entities;

namespace GData {
    public class Class1 {
        public static void Main() {
            DataSource ds = new DataSource(
                "AIzaSyD8mg8Iqd9QZQc-wFs9O8Ud6Hx3ZJ2MSxI",
                "119uJTehNKRJ826lx3rnn2gtW2r0mKxYyWurECNYb22U"
            );
            EntityLoader loader = new EntityLoader(ds);
            GameDataAsset asset = loader.Load(typeof(GameDataAsset));
            var enemies = loader.Load(typeof(Enemy));
            Console.ReadKey();
        }
    }
}