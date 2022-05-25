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
                    //Engine = Engine.Luna,
                    Engine = Engine.Tevian,
                    Name = "Vision Labs Platform",
                    UUID = "92ef5815-8e4d-46f1-aba4-c4f269cb8bf7",
                    IsSelected = true,
                },
                new EngineEntity()
                {
                    //Engine = Engine.Ntech,
                    Engine = Engine.Tevian,
                    Name = "NTech Platform",
                    UUID = "9a948cd1-51e5-479d-9250-46196c0b7e11",
                    IsSelected = false,
                },
                new EngineEntity()
                {
                    Engine = Engine.Tevian,
                    Name = "Tevian Platform",
                    UUID = "38f1a29c-4b3e-49b4-9dec-dfe848a35a36",
                    IsSelected = false,
                },
            };
        }
    }
}
