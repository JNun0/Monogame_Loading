using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Sokoban
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        string levelPath = "Content/SokobanLevels/level1.txt";
        string[] levelsPath = { "Content/SokobanLevels/level1.txt", "Content/SokobanLevels/level2.txt", "Content/SokobanLevels/level3.txt", "Content/SokobanLevels/level4.txt", "Content/SokobanLevels/level5.txt" };
        int currentLevel = 0;
        char[,] map;
        const int Tilesize = 64;
        int width;
        int height;
        List<Vector2> objectivePointPos;
        Texture2D playerTexture, wallTexture, groundTexture, crateTexture, endpointTexture;
        KeyboardManager km = new KeyboardManager();

        delegate bool Verification();
        event Verification OnObjectiveReach;

        //Player
        Vector2 playerPos;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("Character4");
            wallTexture = Content.Load<Texture2D>("Wall_Black");
            groundTexture = Content.Load<Texture2D>("GroundGravel_Grass");
            crateTexture = Content.Load<Texture2D>("Crate_Beige");
            endpointTexture = Content.Load<Texture2D>("EndPoint_Black");

            LoadLevel();

            OnObjectiveReach += IsLevelFinished;
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
 
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            km.Update();
            // TODO: Add your update logic here
            Movement();

            if (km.IsKeyPressed(Keys.R))
            {
                Reset();
            }

            if (IsLevelFinished() == true)
            {
                LoadLevel();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char currentSymbol = map[x, y];
                    switch (currentSymbol)
                    {
                        case 'X':
                            _spriteBatch.Draw(wallTexture,new Vector2(x,y)*Tilesize,Color.White);
                            break;

                        case ' ':
                            _spriteBatch.Draw(groundTexture,new Vector2(x,y)*Tilesize,Color.White);
                            break;

                        case 'B':
                            _spriteBatch.Draw(crateTexture, new Vector2(x, y) * Tilesize, Color.White);
                            break;

                        case '.':
                            _spriteBatch.Draw(endpointTexture, new Vector2(x, y) * Tilesize, Color.White);
                            break;

                        default:
                            _spriteBatch.Draw(groundTexture, new Vector2(x, y) * Tilesize, Color.White);
                            break;

                    }
                }
                _spriteBatch.Draw(playerTexture, playerPos * Tilesize, Color.White);
            }
            // TODO: Add your drawing code here
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void LoadLevel()
        {
            if(currentLevel >= levelsPath.Length)
            {
                return;
            }

            //objectivePointPos.Clear();

            string[] lines = File.ReadAllLines(levelsPath[currentLevel++]);
            map = new char[lines[0].Length,lines.Length];
            objectivePointPos = new List<Vector2>();

            for (int x = 0; x < lines[0].Length; x++)
            {
                for(int y = 0; y < lines.Length; y++)
                {
                    string currentLine = lines[y];
                    map[x,y] = currentLine[x];

                    if (currentLine[x] == '.')
                    {
                        objectivePointPos.Add(new Vector2(x, y));
                    }

                    if(currentLine[x] == 'i')
                    {
                        playerPos = new Vector2(x, y);
                    }
                }
            }

            height = lines.Length;
            width = lines[0].Length;

            _graphics.PreferredBackBufferHeight = height * Tilesize;
            _graphics.PreferredBackBufferWidth = width * Tilesize;
            _graphics.ApplyChanges();


        }

        void Movement()
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

            if(map[(int)newPos.X,(int)newPos.Y] == 'X')
            {
                newPos = playerPos;
            }

            //Box Behaviour
            else if (map[(int)newPos.X, (int)newPos.Y] == 'B')
            {
                if(map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == ' ' || map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == '.')
                {
                    bool isObjective = map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == '.';

                    map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] = 'B';
                    map[(int)(newPos.X), (int)(newPos.Y)] = ' ';
                    
                    foreach(Vector2 pos in objectivePointPos)
                    {
                        if(pos.X == newPos.X + dir.X && pos.Y == newPos.Y + dir.Y)
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
                else if(map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == 'X' || map[(int)(newPos.X + dir.X), (int)(newPos.Y + dir.Y)] == 'B')
                {
                    newPos = playerPos;
                }
                
            }


            map[(int)playerPos.X, (int)playerPos.Y] = ' ';
            playerPos = newPos;
            map[(int)playerPos.X, (int)playerPos.Y] = 'i';
        }

        bool IsLevelFinished()
        {
            foreach(Vector2 pos in objectivePointPos)
            {
                if ((map[(int)pos.X, (int)pos.Y] != 'B'))
                {
                    return false;
                }
            }

            //LoadLevel();

            return true;
        }

        void Reset()
        {
            currentLevel = 0;
            LoadLevel(); 
        }
    }
}
