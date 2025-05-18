import { BrowserRouter } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';
import AppRoutes from './routes';
import './scss/main.scss';

function App() {
  return (
    <BrowserRouter>
      <CssBaseline />
      <AppRoutes />
    </BrowserRouter>
  );
}

export default App;
