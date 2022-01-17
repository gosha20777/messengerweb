using System.Collections.Generic;

namespace MessengerWeb.Client.Services
{
    public class Settings
    {
        public List<EngineEntity> Engines { get; set; }

        public Settings()
        {
            Engines = new List<EngineEntity> 
            {
                new EngineEntity()
                {
                    Engine = Engine.Luna,
                    Name = "Luna Platform",
                    UUID = "63518595-bca0-47ed-805f-72f5c6c350b1",
                    IsSelected = true,
                },
                new EngineEntity()
                {
                    Engine = Engine.Ntech,
                    Name = "NTech Find Face",
                    UUID = "8bd64949-b530-4523-9631-e38545760443",
                    IsSelected = false,
                },
                new EngineEntity()
                {
                    Engine = Engine.Tevian,
                    Name = "Tevian Platform",
                    UUID = "",
                    IsSelected = false,
                }
            };
        }
    }
}
