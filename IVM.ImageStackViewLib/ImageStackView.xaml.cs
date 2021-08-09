using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.WPF;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;

namespace ivm
{
    /// <summary>
    /// StackView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageStackView : UserControl
    {
        public ViewScene scene = null;
        public ViewCamera camera = null;
        public ViewParam param = null;

        OpenGL gl = null;

        public ImageStackView()
        {
            InitializeComponent();

            Loaded += Control_loaded;
        }

        private void Control_loaded(object sender, RoutedEventArgs e)
        {
            RenderTarget.MouseDown += camera.Control_MouseButtonDown;
            RenderTarget.MouseUp += camera.Control_MouseButtonUp;
            RenderTarget.MouseMove += camera.Control_MouseMove;
            RenderTarget.MouseWheel += camera.Control_MouseWheel;
        }

        private void OpenGLControl_Initialized(object sender, OpenGLRoutedEventArgs args)
        {
            camera = new ViewCamera(this);
            param = new ViewParam();

            // init Scene
            gl = args.OpenGL;
            scene = new ViewScene(this);
            scene.Init(gl);
            scene.UpdateModelviewMatrix();
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            scene.UpdateProjectionMatrix();
        }

        private void OpenGLControl_Draw(object sender, OpenGLRoutedEventArgs args)
        {
            scene.Render(gl); // scene render
        }

        public bool Open(string imgPath)
        {
            return scene.Open(gl, imgPath);
        }

        public void UpdateBoxHeight()
        {
            scene.UpdateMesh(gl);
        }
    }
}
