﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.FigurasBasicas;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP.Modelos
{
    class Modelo
    {
        public Matrix MatrizMundo { get; set; }

        public Model MiModelo;

        public Color ColorModelo;

        private Boolean EstaSeleccionado = false;

        private Matrix World;

        private float VelocidadTranslacion;

        public List<Texture2D> TexturasModelo;

        private Texture2D TexturaEnEdicion;

        private int IndexOfTextureToDraw;

        public Modelo(Model modelo, Matrix matriz, Color colorModelo, List<Texture2D> Texturas)
        {
            MatrizMundo = matriz;
            MiModelo = modelo;
            ColorModelo = colorModelo;
            TexturasModelo = Texturas;
        }

        public void Update(GameTime gameTime, Texture2D textura)
        {
            EstaSeleccionado = true;

            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ActualizarPosicion(elapsedTime);

            TexturaEnEdicion = textura;
        }

        public void ActualizarPosicion(float elapsedTime) {
            KeyboardState estadoTeclado = Keyboard.GetState();

            if (estadoTeclado.IsKeyDown(Keys.LeftShift))
                VelocidadTranslacion = elapsedTime * 200;
            else
                VelocidadTranslacion = elapsedTime * 50;

            //TRANSLACIONES
            if (estadoTeclado.IsKeyDown(Keys.D))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Right * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.A))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Left * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.W))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Up * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.S))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Down * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.Q))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Forward * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.E))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Backward * VelocidadTranslacion);

            //ESCALADOS
            if (estadoTeclado.IsKeyDown(Keys.Subtract) && estadoTeclado.IsKeyDown(Keys.LeftShift))
                MatrizMundo = Matrix.CreateScale(elapsedTime * 50) * MatrizMundo;

            if (estadoTeclado.IsKeyDown(Keys.Add) && estadoTeclado.IsKeyDown(Keys.LeftShift))
                MatrizMundo = Matrix.CreateScale(1 / (elapsedTime * 50)) * MatrizMundo;


            //ROTACIONES
            if (estadoTeclado.IsKeyDown(Keys.Z)) {
                if (estadoTeclado.IsKeyDown(Keys.Add))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, elapsedTime))
                    * MatrizMundo;
                if (estadoTeclado.IsKeyDown(Keys.Subtract))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -elapsedTime))
                    * MatrizMundo;
            }



            if (estadoTeclado.IsKeyDown(Keys.X)) {
                if (estadoTeclado.IsKeyDown(Keys.Add))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, elapsedTime))
                    * MatrizMundo;
                if (estadoTeclado.IsKeyDown(Keys.Subtract))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, -elapsedTime))
                    * MatrizMundo;
            }

            if (estadoTeclado.IsKeyDown(Keys.C)) {
                if (estadoTeclado.IsKeyDown(Keys.Add))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitY, elapsedTime))
                    * MatrizMundo;
                if (estadoTeclado.IsKeyDown(Keys.Subtract))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -elapsedTime))
                    * MatrizMundo;
            }
        }

        public void Draw(Effect efecto)
        {
            IndexOfTextureToDraw = 0;

            if (EstaSeleccionado)
                DibujarConOtraTextura(efecto);

            else 
                DibujarNormalmente(efecto);

            EstaSeleccionado = false;
        }

        public void DibujarNormalmente(Effect efecto) {
            efecto.Parameters["DiffuseColor"]?.SetValue(ColorModelo.ToVector3());

            foreach (var mesh in MiModelo.Meshes)
            {
                World = mesh.ParentBone.Transform * MatrizMundo;

                efecto.Parameters["World"].SetValue(World);
                efecto.Parameters["ModelTexture"].SetValue(TexturasModelo[IndexOfTextureToDraw++]);
                mesh.Draw();
            }
        }

        public void DibujarConOtraTextura(Effect efecto) {
            efecto.Parameters["DiffuseColor"]?.SetValue(Color.Magenta.ToVector3());

            foreach (var mesh in MiModelo.Meshes)
            {
                World = mesh.ParentBone.Transform * MatrizMundo;

                efecto.Parameters["World"].SetValue(World);
                efecto.Parameters["ModelTexture"].SetValue(TexturaEnEdicion);
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
                + "VECTOR FOWARD: " + vectorFoward;
            //return this.MatrizMundo.ToString().Replace('{', '\n').Replace('}', ' ');
        }
    }

}
