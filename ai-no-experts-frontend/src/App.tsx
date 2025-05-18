import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';

import AppRoutes from './routes';

import './scss/main.scss';

function App() {
  return (
    <React.Fragment>
      <CssBaseline />
      <Router>
        <AppRoutes />
      </Router>
    </React.Fragment>
  );
}

export default App;
