﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.FigurasBasicas;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP.Modelos
{


    class NormalCar
    {
        private Effect Effect { get; set; }
        public Matrix MatrizMundo { get; set; }
        public Model CarModel { get; set; }

        public Color ColorModelo;

        private Boolean EstaSeleccionado = false;

        private Matrix World;

        private float Velocidad;
        public Texture2D CarTexture { set; get; }

        private Texture2D TexturaEnEdicion;

        private float VelocidadBase;

        private float Tiempo;

        private float Aceleracion;
        private ContentManager ContentManager { get; set; }


        public NormalCar(ContentManager content, Matrix WorldMatrix, Color ColorModel) {
            ContentManager = content;
            MatrizMundo = WorldMatrix;
            ColorModelo = ColorModel;

            LoadContent();

            Velocidad = 0;
            VelocidadBase = 10;
            Aceleracion = 50;
            Tiempo = 0;

           
            foreach (var mesh in CarModel.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }
        }


        public void Update(GameTime gameTime, Texture2D textura)
        {
            EstaSeleccionado = true;

            Update(gameTime);

            TexturaEnEdicion = textura;
        }

        public void Update(GameTime gameTime) {
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState estadoTeclado = Keyboard.GetState();

            if (estadoTeclado.IsKeyDown(Keys.LeftShift) && estadoTeclado.IsKeyDown(Keys.W))
            {
                Velocidad += Aceleracion * elapsedTime;
                Velocidad = MathHelper.Clamp(Velocidad, 0, 80);
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Backward * Velocidad);
                TransformarMatrizRueda(Matrix.CreateRotationX(MathHelper.Pi / 12 * Velocidad));
            }
            else if (estadoTeclado.IsKeyDown(Keys.W))
            {
                Velocidad = 10;
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Backward * Velocidad);
                TransformarMatrizRueda(Matrix.CreateRotationX(MathHelper.Pi / 12 * Velocidad));
            }
            else {
                Velocidad -= Aceleracion * elapsedTime;
                Velocidad = MathHelper.Clamp(Velocidad, 0, 80);
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Backward * Velocidad);
                TransformarMatrizRueda(Matrix.CreateRotationX(MathHelper.Pi / 12 * Velocidad));
            }


            if (estadoTeclado.IsKeyDown(Keys.LeftControl) && !estadoTeclado.IsKeyDown(Keys.LeftShift)) {
                Velocidad += Aceleracion * elapsedTime;
            }


            //TRANSLACIONES
            if (estadoTeclado.IsKeyDown(Keys.D)) {
                MatrizMundo = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -elapsedTime)) * MatrizMundo;
                //TransformarRuedasDelanteras(Matrix.CreateRotationY(MathHelper.Pi / 26));
            }

            if (estadoTeclado.IsKeyDown(Keys.A)) {
                MatrizMundo = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitY, elapsedTime)) * MatrizMundo;
                //TransformarRuedasDelanteras(Matrix.CreateRotationY(-MathHelper.Pi / 26));
            }
                
            if (estadoTeclado.IsKeyDown(Keys.S)) {
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Forward * VelocidadBase);
                TransformarMatrizRueda(Matrix.CreateRotationX(- MathHelper.Pi / 12 * Velocidad));
            }
               
        }

        public void Draw()
        {
            if (EstaSeleccionado)
                DibujarConOtraTextura();

            else 
                DibujarNormalmente();

            EstaSeleccionado = false;
        }

        public void DibujarNormalmente() {
            Effect.Parameters["ModelTexture"].SetValue(CarTexture);

            foreach (var mesh in CarModel.Meshes)
            {
                World = mesh.ParentBone.Transform * MatrizMundo;

                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }

        }

        public void DibujarConOtraTextura() {
            Effect.Parameters["DiffuseColor"]?.SetValue(Color.Magenta.ToVector3());
            Effect.Parameters["ModelTexture"].SetValue(CarTexture);

            foreach (var mesh in CarModel.Meshes)
            {
                World = mesh.ParentBone.Transform * MatrizMundo;

                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }
        }

        public Matrix getWorldMatrix()
        {
            return this.MatrizMundo;
        }

        public String getWorldMatrixAsString()
        {
            String posicion = MatrizMundo.Translation.ToString();
            String vectorUp = MatrizMundo.Up.ToString();
            String vectorFoward = MatrizMundo.Forward.ToString();
            return "POSICION: " + posicion + '\n'
                + "VECTOR UP: " + vectorUp + '\n'
                + "VECTOR FOWARD: " + vectorFoward + '\n'
                + "VELOCIDAD ACTUAL: " + Velocidad;
            //return this.MatrizMundo.ToString().Replace('{', '\n').Replace('}', ' ');
        }


        private void LoadContent() {
            CarModel = ContentManager.Load<Model>("Models/cars/RacingCar");
            Effect = ContentManager.Load<Effect>("Effects/BasicShader");
            CarTexture = ContentManager.Load<Texture2D>("Textures/NormalCarTextures/Vehicle_basecolor");
        }

        private void TransformarMatrizRueda(Matrix transformacion) {
            CarModel.Bones[2].Transform = transformacion * CarModel.Bones[2].Transform;
            CarModel.Bones[3].Transform = transformacion * CarModel.Bones[3].Transform;
            CarModel.Bones[4].Transform = transformacion * CarModel.Bones[4].Transform;
            CarModel.Bones[5].Transform = transformacion * CarModel.Bones[5].Transform;
        }

        private void TransformarRuedasDelanteras(Matrix transformacion)
        {
            CarModel.Bones[2].Transform = transformacion * CarModel.Bones[2].Transform;
            CarModel.Bones[3].Transform = transformacion * CarModel.Bones[3].Transform;
        }
    }

}
