using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Involved.HTF.Common.Entities
{
    public class ShipSub
    {
        public int DepthPerMeter = 0;
        public int Depth { get; set; } = 0;
        public int Distance { get; set; } = 0;

        public void ExecuteCommand(string command)
        {
            var commandsList = command
                .Split(' ')
                .Select(command => command.Trim())
                .ToList();

            string text = commandsList[0];
            int movementAmount = int.Parse(commandsList[1]);

            switch (text)
            {
                case "Down":
                    DepthPerMeter += movementAmount;
                    break;
                case "Up":
                    DepthPerMeter -= movementAmount;
                    break;
                case "Forward":
                    Depth = Depth * movementAmount;
                    Distance += movementAmount;
                    break;
            }
        }
    }


}
