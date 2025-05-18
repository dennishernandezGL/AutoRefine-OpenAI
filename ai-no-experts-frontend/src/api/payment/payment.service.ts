import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: '',
        environment: process.env.NODE_ENV || 'development',
        instanceIdentifier: generateInstanceIdentifier() // replace with an actual implementation
      },
      data
    );
    
    const response = await axios.post('/api/log/LogInfo', logInfoRequest);
  
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

function generateInstanceIdentifier() {
  // Implement this function to return a unique identifier for the instance
  return "some-unique-identifier";
}