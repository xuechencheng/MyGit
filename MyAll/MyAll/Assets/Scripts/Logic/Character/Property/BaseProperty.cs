using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class BaseProperty : MonoBehaviour
    {
        public PropertyValue<string> Name = new PropertyValue<string>();
    }
}