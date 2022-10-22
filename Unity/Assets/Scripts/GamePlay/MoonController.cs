﻿using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SunAndMoon
{
    public class MoonController : RoleController<MoonController>
    {
        protected async override UniTask Awake()
        {
            await base.Awake();
            var targetPrefab = await Resources.LoadAsync<GameObject>("Prefabs/SunTarget");
            var instance = Instantiate(targetPrefab) as GameObject;
            instance.transform.position = new Vector3(transform.position.x, instance.transform.position.y, transform.position.z);
        }
    }
}