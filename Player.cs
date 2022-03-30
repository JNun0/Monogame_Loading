using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{
    class Player
    {
        Vector2 playerPos, dir, newPos;
        KeyboardManager km = new KeyboardManager();
        char[,] map;
        List<Vector2> objectivePointPos;
        delegate bool Verification();
        event Verification OnObjectiveReach;

        public Player()
        {
            newPos = playerPos;
            dir = Vector2.Zero;
        }

        public void Movement()
        {

            Vector2 newPos = playerPos;
            Vector2 dir = Vector2.Zero;

            if (km.IsKeyPressed(Keys.W))
            {
                newPos -= Vector2.UnitY;
                dir = -Vector2.UnitY;
            }
            if (km.IsKeyPressed(Keys.A))
            {
                newPos -= Vector2.UnitX;
                dir = -Vector2.UnitX;
            }
            if (km.IsKeyPressed(Keys.S))
            {
                newPos += Vector2.UnitY;
                dir = Vector2.UnitY;
            }
            if (km.IsKeyPressed(Keys.D))
            {
                newPos += Vector2.UnitX;
                dir = Vector2.UnitX;
            }

            if (map[(int)newPos.X, (int)newPos.Y] == 'X')
            {
                newPos = playerPos;
            }

            //Box Behaviour
            else if (map[(int)newPos.X, (int)newPos.Y] == 'B')
            {
                if (map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == ' ' || map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == '.')
                {
                    bool isObjective = map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == '.';

                    map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] = 'B';
                    map[(int)(newPos.X), (int)(newPos.Y)] = ' ';

                    foreach (Vector2 pos in objectivePointPos)
                    {
                        if (pos.X == newPos.X + dir.X && pos.Y == newPos.Y + dir.Y)
                        {
                            OnObjectiveReach?.Invoke();
                            break;
                        }
                    }


                    if (isObjective)
                    {
                        //? - se for diferente de null
                        OnObjectiveReach?.Invoke();
                    }
                }
                else if (map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == 'X' || map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == 'B')
                {
                    newPos = playerPos;
                }

            }


            map[(int)playerPos.X, (int)playerPos.Y] = ' ';
            playerPos = newPos;
            map[(int)playerPos.X, (int)playerPos.Y] = 'i';
        }
    }
}
