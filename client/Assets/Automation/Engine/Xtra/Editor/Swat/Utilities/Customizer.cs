using System.Collections.Generic;

namespace TrilleonAutomation {

    public class Customizer : ConfigReader {

        public static Customizer Self { get {
                if(_self == null) {
                    _self = new Customizer();
                }
                return _self;
            } 
        }
        static Customizer _self;
        public Customizer() : base(FileResource.CustomizationConfig, false) { }

    }

}