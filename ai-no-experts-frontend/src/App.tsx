import { BrowserRouter } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';
import { useEffect } from 'react';
import mixpanel from 'mixpanel-browser';

import AppRoutes from './routes';

import './scss/main.scss';

function App() {
  useEffect(() => {
    mixpanel.init('YOUR_PROJECT_TOKEN', { debug: true });
    mixpanel.track('App Mounted');
  }, []);

  return (
    <>
      <CssBaseline />
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </>
  );
}

export default App;