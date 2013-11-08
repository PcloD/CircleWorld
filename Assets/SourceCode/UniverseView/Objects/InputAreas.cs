using UnityEngine;

public class InputAreas
{
    static private Rect[] inputAreas = new Rect[10];
    static private int inputAreasCount;
    
        
    static public void ResetInputAreas()
    {
        inputAreasCount = 0;
    }
    
    static public void AddInputArea(Rect rect)
    {
        inputAreas[inputAreasCount++] = rect;
    }

    static public bool IsInputArea(Vector2 inputPosition)
    {
        //Convert from Input coordinate system to GUI coordinate system
        inputPosition.y = Screen.height - inputPosition.y;
        
        for (int i = 0; i < inputAreasCount; i++)
            if (inputAreas[i].Contains(inputPosition))
                return true;
        
        return false;
    }
}


