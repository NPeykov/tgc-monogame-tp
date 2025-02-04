﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.FigurasBasicas;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal  del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Descomentar para que el juego sea pantalla completa.
            // Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private SpriteFont spriteFont { get; set; }
        private Model CarModel { get; set; }
        private BattleCar BattleCar { get; set; }
        private Effect Effect { get; set; }
        private Effect TilingEffect { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }
        private QuadPrimitive quad { get; set; }
        private String ubicacionModelo { get; set; }
        private TipoDeCamara tipoDeCamara { get; set; }
        private Texture2D FloorTexture { get; set; }
        private List<Texture2D> CarTextures { get; set; }
        private List<Texture2D> BattleCarTextures { get; set; }
        private Texture2D TexturaDeAutoEnEdicion;
        private int IndexListaModelo { get; set; }
        private int CantidadModelos { get; set; }
        private FollowCamera CamaraAutoPrincipal { get; set; }
        private NormalCar AutoPrincipal { get; set; }
        private List<NormalCar> ModelosUsados { get; set; }
        private List<Pared> Walls { get; set; }
        private Texture2D WallTexture { get; set; }
        private Matrix viewProjection;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            // Apago el backface culling.
            // Esto se hace por un problema en el diseno del modelo del logo de la materia.
            // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;


            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);
            
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f);

            CamaraAutoPrincipal = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);

            tipoDeCamara = TipoDeCamara.ORIGINAL_SCENE;
           

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            CarModel = Content.Load<Model>(ContentFolder3D + "cars/RacingCar");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            TilingEffect = Content.Load<Effect>(ContentFolderEffects + "TextureTiling");
            TilingEffect.Parameters["Tiling"].SetValue(new Vector2(10f, 10f));

            spriteFont = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "Arial");

            FloorTexture = Content.Load<Texture2D>(ContentFolderTextures + "grass");

            TexturaDeAutoEnEdicion = Content.Load<Texture2D>(ContentFolderTextures + "ground");

            WallTexture = Content.Load<Texture2D>(ContentFolderTextures + "stones");


            InicializarAutos();
            InicializarParedes();

            quad = new QuadPrimitive(GraphicsDevice);

            viewProjection = new Matrix();

            CantidadModelos = ModelosUsados.Count;
            IndexListaModelo = 0;

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            KeyboardState estadoTeclado = Keyboard.GetState();

            FreeCamera.Update(gameTime);

            if (estadoTeclado.IsKeyDown(Keys.Escape))
                Exit();

            if (estadoTeclado.IsKeyDown(Keys.F1))
                tipoDeCamara = TipoDeCamara.ORIGINAL_SCENE;

            if (estadoTeclado.IsKeyDown(Keys.F2))
                tipoDeCamara = TipoDeCamara.FREE_VIEW;

            if (estadoTeclado.IsKeyDown(Keys.F3))
                tipoDeCamara = TipoDeCamara.FOLLOW_CAMERA;

            if (estadoTeclado.IsKeyDown(Keys.M)) {
                if (CantidadModelos - 1 > IndexListaModelo)
                    IndexListaModelo++;
                else
                    IndexListaModelo = 0;
            }

            //ModelosUsados[IndexListaModelo].Update(gameTime, TexturaDeAutoEnEdicion);
            AutoPrincipal.Update(gameTime);

            CamaraAutoPrincipal.Update(gameTime, AutoPrincipal.getWorldMatrix());
                
            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //Actualizo projeccion segun tipo de camara
            if (tipoDeCamara == TipoDeCamara.ORIGINAL_SCENE) {
                Effect.Parameters["View"].SetValue(TargetCamera.View);
                Effect.Parameters["Projection"].SetValue(TargetCamera.Projection);
                viewProjection = TargetCamera.View * TargetCamera.Projection;
            }

            if (tipoDeCamara == TipoDeCamara.FREE_VIEW) {
                Effect.Parameters["View"].SetValue(FreeCamera.View);
                Effect.Parameters["Projection"].SetValue(FreeCamera.Projection);
                viewProjection = FreeCamera.View * FreeCamera.Projection;
            }

            if (tipoDeCamara == TipoDeCamara.FOLLOW_CAMERA) {
                Effect.Parameters["View"].SetValue(CamaraAutoPrincipal.View);
                Effect.Parameters["Projection"].SetValue(CamaraAutoPrincipal.Projection);
                viewProjection = CamaraAutoPrincipal.View * CamaraAutoPrincipal.Projection;
            }

            TilingEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateScale(2000) * viewProjection);
            TilingEffect.Parameters["Texture"].SetValue(FloorTexture);
            quad.Draw(TilingEffect);


            foreach (var wall in Walls) {
                TilingEffect.Parameters["Texture"].SetValue(WallTexture);
                TilingEffect.Parameters["WorldViewProjection"].SetValue(wall.getWorldMatrix() * viewProjection);
                quad.Draw(TilingEffect);
            }

            foreach (var auto in ModelosUsados)
            {
                auto.Draw();
            }

            AutoPrincipal.Draw();

            BattleCar.Draw();

            //Esto es para escribir el texto que aparece en la pantalla con la ubicacion del modelo
            ubicacionModelo = AutoPrincipal.getWorldMatrixAsString();

            SpriteBatch.Begin();
            SpriteBatch.DrawString(spriteFont, "MODELO ACTUAL: Auto Principal"
                + '\n' + ubicacionModelo, new Vector2(0, 0), Color.Magenta);
            SpriteBatch.End();

            
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

        //FUNCIONES PARA SACAR LOGICA DE INITIALIZE

        private void InicializarAutos() {
            ModelosUsados = new List<NormalCar>();

            var rotacion = Quaternion.CreateFromAxisAngle(-Vector3.UnitY, MathHelper.Pi / 3);
            var matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(135, 0, -321);
            AutoPrincipal = new NormalCar(Content, matrizInicial, Color.Red);


            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 3);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-374, 0f, -523);
            ModelosUsados.Add(new NormalCar(Content, matrizInicial, Color.White));


            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 8);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(179f, 0f, -77f);
            ModelosUsados.Add(new NormalCar(Content, matrizInicial, Color.Red));


            
            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 2);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(115f, 0f, -533f);
            ModelosUsados.Add(new NormalCar(Content, matrizInicial, Color.Blue));

            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 8);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-81f, 0f, 159f);
            ModelosUsados.Add(new NormalCar(Content, matrizInicial, Color.Green));

            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 2);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-80f, 0f, -160f);
            ModelosUsados.Add(new NormalCar(Content, matrizInicial, Color.GreenYellow));
            
            matrizInicial = Matrix.CreateScale(0.01f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-200f, 0f, -120f);
            BattleCar = new BattleCar(Content, matrizInicial, Color.Orange);
        }

        private void InicializarParedes() {
            Walls = new List<Pared>();
            var PosNuevaPared = new Matrix();
            var BoundingBox = new BoundingBox();
            var minVector = Vector3.One * 0.25f;

            var scale = new Vector3(500f, 1f, 200f);
            
            //********** Paredes plano XY en Z=-2000, yendo de X=-2000 a X=2000 ****************//
            //Pared 1:      |**|  |  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-1500f, 200f, -2000f));
            BoundingBox = new BoundingBox(new Vector3(-2000f, 0f, -2000f) - minVector, new Vector3(-1000f, 200f, -2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 2:      |  |**|  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-500f, 200f, -2000f));
            BoundingBox = new BoundingBox(new Vector3(-1000f, 0f, -2000f) - minVector, new Vector3(0f, 200f, -2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 3:      |  |  |**|  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(500f, 200f, -2000f));
            BoundingBox = new BoundingBox(new Vector3(0f, 0f, -2000f) - minVector, new Vector3(1000f, 200f, -2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 4:      |  |  |  |**|
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(1500f, 200f, -2000f));
            BoundingBox = new BoundingBox(new Vector3(1000f, 0f, -2000f) - minVector, new Vector3(2000f, 200f, -2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));


            //********** Paredes plano XY en Z=2000, yendo de X=-2000 a X=2000 ****************//
            //Pared 5:      |**|  |  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-1500f, 200f, 2000f));
            BoundingBox = new BoundingBox(new Vector3(-2000f, 0f, 2000f) - minVector, new Vector3(-1000f, 200f, 2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 6:      |  |**|  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-500f, 200f, 2000f));
            BoundingBox = new BoundingBox(new Vector3(-1000f, 0f, 2000f) - minVector, new Vector3(0f, 200f, 2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 7:      |  |  |**|  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(500f, 200f, 2000f));
            BoundingBox = new BoundingBox(new Vector3(0f, 0f, 2000f) - minVector, new Vector3(1000f, 200f, 2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 8:      |  |  |  |**|
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(1500f, 200f, 2000f));
            BoundingBox = new BoundingBox(new Vector3(1000f, 0f, 2000f) - minVector, new Vector3(2000f, 200f, 2000f) + minVector);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));



            scale = new Vector3(200f, 1f, 500f);

            //********** Paredes plano YZ en X=-2000, yendo de Z=-2000 a Z=2000 ****************//
            //Pared 9:      |**|  |  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-2000f, 200f, -1500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 10:      |  |**|  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-2000f, 200f, 500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 11:      |  |  |**|  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-2000f, 200f, -500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 12:      |  |  |  |**|
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-2000f, 200f, 1500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));


            //********** Paredes plano YZ en X=2000, yendo de Z=-2000 a Z=2000 ****************//
            //Pared 13:      |**|  |  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(2000f, 200f, -1500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 14:      |  |**|  |  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(2000f, 200f, -500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 15:      |  |  |**|  |
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(2000f, 200f, 500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            //Pared 16:      |  |  |  |**|
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(2000f, 200f, 1500f));
            Walls.Add(new Pared(PosNuevaPared, WallTexture));
        }

    }
}