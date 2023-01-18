import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Home } from "./components/Home";
import {Stats} from "./components/Stats";
import {Game} from "./components/Game";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/stats',
    requireAuth: true,
    element: <Stats/>
  },
  {
    path: '/game',
    requireAuth: true,
    element: <Game/>
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
