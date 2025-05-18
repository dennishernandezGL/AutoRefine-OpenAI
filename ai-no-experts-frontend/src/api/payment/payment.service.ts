import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: 'user123', // Example hardcoded value; should be dynamic
        environment: 'production', // Example environment setting
        instanceIdentifier: 'instance123' // Example ID; should be unique
      },
      data
    );
    
    const response = await axios.post('https://localhost:5050/api/log/LogInfo', logInfoRequest);
  
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