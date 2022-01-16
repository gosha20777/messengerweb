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
                    Name = "Luna Platform",
                    Id = "63518595-bca0-47ed-805f-72f5c6c350b1",
                    IsSelected = true,
                },
                new EngineEntity()
                {
                    Name = "NTech Find Face",
                    Id = "bla-bla",
                    IsSelected = false,
                },
                new EngineEntity()
                {
                    Name = "Tevian Platform",
                    Id = "",
                    IsSelected = false,
                }
            };
        }
    }
}
