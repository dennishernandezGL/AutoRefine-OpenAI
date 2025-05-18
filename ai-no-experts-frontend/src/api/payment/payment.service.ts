import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: 'user123', // Add user identifier
        environment: process.env.NODE_ENV || 'development', // Use environment variables for configuration
        instanceIdentifier: 'instance42' // Use a real identifier for logging tracking
      },
      data
    );
    
    const response = await axios.post('/api/log/LogInfo', logInfoRequest); // Use relative path to avoid hard-coded URLs
  
    return {
      statusCode: response.status,
      data: response.data,
    };
  } catch (error: any) {
    console.error('submitPayment error:', error);
    
    return {
      statusCode: error?.response?.status || 500,
      data: error?.response?.data || { error: 'Unknown error occurred' },
    };
  }  
};