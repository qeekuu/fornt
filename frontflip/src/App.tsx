import './App.css'
import {createBrowserRouter, RouterProvider} from "react-router-dom";
import Root from "./Root.tsx";
import HomePage from "./pages/HomePage/HomePage.tsx";
import AnimatedPage from "./components/AnimatedPage.tsx";
import LoginPage from './pages/LoginPage/LoginPage.tsx';
import RegisterPage from './pages/LoginPage/RegisterPage.tsx';
import AppLayout from './AppLayout.tsx';
import GalleryPage from './pages/GalleryPage/GalleryPage.tsx';
import AddPhotoPage from './pages/AddPhotoPage/AddPhotoPage.tsx';

const router = createBrowserRouter([
  {
    path: "/",
    Component: Root,
    children: [
      {
        path: "about",
        Component: () => <AnimatedPage>
          <h1>About Frontflip</h1>
          <p>Frontflip is a React framework for building blazing fast websites with the power of static site generation
            and server-side rendering.</p>
        </AnimatedPage>
      },
      {
		index: true,
        Component: LoginPage,
      },
      {
        path: "/register",
        Component: RegisterPage,
      },
 
    ],
  },
 {
    path: "/",
    Component: AppLayout,
    children: [
      {
		  path: "\home",
		  Component: HomePage,
      },
	  {
		path: "\gallery",
		Component: GalleryPage,
	  },
	  {
		path: "\addPhoto",
		Component: AddPhotoPage,
	  }
    ],
  },

])

function App() {
  return <RouterProvider router={router}/>
}

export default App
