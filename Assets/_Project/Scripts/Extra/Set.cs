using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set 
{
    public List<Vector3> set = new List<Vector3>();

    public bool Add(Vector3 element){
        if (!set.Contains(element)){
            set.Add(element);
            return true;
        }
        else
        {
            return false;
        }
    }


}
