using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Behaviors connected to Emitters subscribe to this interface to define interaction logic for products of emitters, fluxes and molecules
public interface IEmitter
{
    void TriggerEnter(Emitter.EmitterPackage ep);
    void TriggerExit(Emitter.EmitterPackage ep);
}
