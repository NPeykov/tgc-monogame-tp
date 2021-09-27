﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
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
        private Model battleCarModel { get; set; }
        private Effect Effect { get; set; }
        //private Matrix World { get; set; }
        //private Matrix View { get; set; }
        //private Matrix Projection { get; set; }
        private float Rotation { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }
        private QuadPrimitive quad { get; set; }
        private String ubicacionModelo { get; set; }
        private TipoDeCamara tipoDeCamara { get; set; }
        private List<Modelo> AutosNormales { get; set; }

        private int IndexListaModelo { get; set; }

        private int CantidadModelos { get; set; }

        private List<Matrix> UbicacionesAutosComunes;
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
            // Seria hasta aca.

            // Configuramos nuestras matrices de la escena.
            //World = Matrix.Identity;
            //View = Matrix.CreateLookAt(Vector3.UnitZ, Vector3.Zero, Vector3.Up);
            //Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            UbicacionesAutosComunes = new List<Matrix>();
            AutosNormales = new List<Modelo>();

            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f);

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
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            

            CarModel = Content.Load<Model>(ContentFolder3D + "cars/RacingCar");

            battleCarModel = Content.Load<Model>(ContentFolder3D + "CombatVehicle/Vehicle");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            spriteFont = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "Arial");


            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.
            foreach (var mesh in CarModel.Meshes)
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                     meshPart.Effect = Effect;

            foreach (var mesh in battleCarModel.Meshes)
                foreach (var meshPart in mesh.MeshParts)
                    meshPart.Effect = Effect;


            //cargo los modelos de los autos comunes en sus posiciones iniciales 
            //a una lista de matrices de mundo
            var rotacion = Quaternion.CreateFromAxisAngle(-Vector3.UnitY, MathHelper.Pi / 3);
            var matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(100f, 0f, -100f);
            //UbicacionesAutosComunes.Add(matrizInicial);
            AutosNormales.Add(new AutoNormal(CarModel, matrizInicial, Color.Red));

            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 3);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-100f, 0f, -100f);
            //UbicacionesAutosComunes.Add(matrizInicial);
            AutosNormales.Add(new AutoNormal(CarModel, matrizInicial, Color.White));

            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.Pi / 8);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(0f, 0f, -100f);
            //UbicacionesAutosComunes.Add(matrizInicial);
            AutosNormales.Add(new AutoNormal(CarModel, matrizInicial, Color.Red));

            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 2);
            matrizInicial = Matrix.CreateScale(0.4f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(0f, -80f, -100f);
            //UbicacionesAutosComunes.Add(matrizInicial);
            AutosNormales.Add(new AutoNormal(CarModel, matrizInicial, Color.Blue));

            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 8);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(0f, 50f, -100f);
            //UbicacionesAutosComunes.Add(matrizInicial);
            AutosNormales.Add(new AutoNormal(CarModel, matrizInicial, Color.Green));


            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 2);
            matrizInicial = Matrix.CreateScale(0.1f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-80f, 30f, -100f);
            //UbicacionesAutosComunes.Add(matrizInicial);
            AutosNormales.Add(new AutoNormal(CarModel, matrizInicial, Color.GreenYellow));

            quad = new QuadPrimitive(GraphicsDevice);

            CantidadModelos = AutosNormales.Count;

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

            if (estadoTeclado.IsKeyDown(Keys.M)) {
                if (CantidadModelos - 1 > IndexListaModelo)
                    IndexListaModelo++;
                else
                    IndexListaModelo = 0;
            }

            AutosNormales[IndexListaModelo].Update(gameTime);

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

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

            //Actualizo projeccion segun tipo de camara
            if (tipoDeCamara == TipoDeCamara.ORIGINAL_SCENE) {
                Effect.Parameters["View"].SetValue(TargetCamera.View);
                Effect.Parameters["Projection"].SetValue(TargetCamera.Projection);
            }

            if (tipoDeCamara == TipoDeCamara.FREE_VIEW) {
                Effect.Parameters["View"].SetValue(FreeCamera.View);
                Effect.Parameters["Projection"].SetValue(FreeCamera.Projection);
            }



            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkRed.ToVector3());


            foreach (var auto in AutosNormales)
            {
                auto.Draw(Effect);
            }
            /*
            foreach (var ubicacion in UbicacionesAutosComunes) {
                foreach (var mesh in CarModel.Meshes)
                {
                    World = mesh.ParentBone.Transform * ubicacion;

                    Effect.Parameters["World"].SetValue(World);
                    mesh.Draw();
                }
            }*/

            Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());

            //quad.Draw(World, View, Projection);
            quad.Draw(Effect);


            //se pudo cargar el modelo del auto de batalla pero no lo puedo transladar
            //bien al frustum

            /*
            foreach (var mesh in battleCarModel.Meshes)
            {
                World = mesh.ParentBone.Transform *
                    Matrix.CreateScale(0.1f) *
                    Matrix.CreateTranslation(-200f, -450f, -300f);

                Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());
                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }*/


            //ubicacionModelo = UbicacionesAutosComunes[0].Translation.ToString();

            ubicacionModelo = AutosNormales[IndexListaModelo].getWorldMatrixAsString();

            SpriteBatch.Begin();
            SpriteBatch.DrawString(spriteFont, "modelo actual: " + ubicacionModelo, new Vector2(0, 0), Color.Red);
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

    }
}