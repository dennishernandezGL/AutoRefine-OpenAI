import { BrowserRouter as Router } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';

import AppRoutes from './routes';

import './scss/main.scss';

function App() {
  return (
    <>
      <CssBaseline />
      <Router basename="/app">
        <AppRoutes />
      </Router>
    </>
  );
}

export default App;
