using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : ScriptableObject {
    public Sprite SkillIcon;
    public List<GameObject> ObjToCreate;
    public float time;
    public bool isPlayAnim;
}
