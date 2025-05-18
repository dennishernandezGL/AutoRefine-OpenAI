import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: '', // It would be good practice to capture the user ID if available
        environment: process.env.NODE_ENV || 'development', // Set the environment dynamically
        instanceIdentifier: generateInstanceId() // Assuming a function to generate an instance identifier
      },
      data
    );
    
    const response = await axios.post('/api/log/LogInfo', logInfoRequest); // Use relative URL
  
    return {
      statusCode: response.status,
      data: response.data,
    };
  } catch (error: any) {
    console.error('submitPayment error:', error.toString()); // More secure and less verbose error logging
    
    return {
      statusCode: error?.response?.status || 500,
      data: error?.response?.data || { error: 'Unknown error occurred' },
    };
  }  
};

function generateInstanceId() {
  // A simple example function to generate an instance identifier
  return Math.random().toString(36).substr(2, 9);
}