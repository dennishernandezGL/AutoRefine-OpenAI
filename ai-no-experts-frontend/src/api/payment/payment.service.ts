import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: 'ANON', // Provide default or actual username
        environment: process.env.NODE_ENV || 'development', // Set environment to actual or default
        instanceIdentifier: 'instanceID1' // Set your unique instance ID
      },
      data
    );
    
    const response = await axios.post('/api/log/LogInfo', logInfoRequest); // Changed to relative URL

    return {
      statusCode: response.status,
      data: response.data,
    };
  } catch (error: any) {
    console.error('Unable to process the payment:', error);
    
    return {
      statusCode: error?.response?.status || 500,
      data: error?.response?.data || { error: 'Unknown error occurred' },
    };
  }  
};