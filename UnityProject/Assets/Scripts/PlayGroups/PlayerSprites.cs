﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayGroup
{
    public class PlayerSprites : NetworkBehaviour
    {
        private readonly Dictionary<string, ClothingItem> clothes = new Dictionary<string, ClothingItem>();
        [SyncVar(hook = "FaceDirection")] public Vector2 currentDirection;

        private bool initSync;
        public PlayerMove playerMove;

        private void Awake()
        {
            foreach (ClothingItem c in GetComponentsInChildren<ClothingItem>())
            {
                clothes[c.name] = c;
            }
        }

        public override void OnStartServer()
        {
            FaceDirection(Vector2.down);
            base.OnStartServer();
        }

        public override void OnStartClient()
        {
            StartCoroutine(WaitForLoad());
            base.OnStartClient();
        }

        private IEnumerator WaitForLoad()
        {
            yield return new WaitForSeconds(2f);
            FaceDirection(currentDirection);
        }

        public void AdjustSpriteOrders(int offsetOrder)
        {
            foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
            {
                int newOrder = s.sortingOrder;
                newOrder += offsetOrder;
                s.sortingOrder = newOrder;
            }
        }

        [Command]
        public void CmdChangeDirection(Vector2 direction)
        {
            FaceDirection(direction);
        }

        //turning character input and sprite update
        public void FaceDirection(Vector2 direction)
        {
            SetDir(direction);
        }

        public void SetDir(Vector2 direction)
        {
            if (playerMove.isGhost)
            {
                return;
            }

            if (currentDirection != direction || !initSync)
            {
                if (!initSync)
                {
                    initSync = true;
                }

                foreach (ClothingItem c in clothes.Values)
                {
                    c.Direction = direction;
                }


                //If both values are set, we're trying to face a diagonal sprite direction, which doesn't exist. To resolve, randomly select one of the appropriate cardinal directions.
                if (direction.x != 0 && direction.y != 0)
                {
                    switch (Random.Range(0, 1))
                    {
                        case 0:
                            direction.x = 0;
                            break;
                        case 1:
                            direction.y = 0;
                            break;
                    }
                }

                currentDirection = direction;
            }
        }
    }
}