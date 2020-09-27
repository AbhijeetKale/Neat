using System;
using Neat.Config;
namespace Neat.Framework
{
    public class NeatMain
    {
        public static NeatConfig config = new NeatConfig();

        protected int innovationNumber = 1;

        public NeatMain() {
            
        }

        public NeatMain(NeatConfig config)
        {
            NeatMain.config = config;
        }
    }
}
