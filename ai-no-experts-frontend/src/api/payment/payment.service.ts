import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    // Exclude sensitive data before logging
    const { creditCardNumber, cvv, ...safeData } = data;

    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: process.env.LOGGER_USER || 'defaultUser',
        environment: process.env.NODE_ENV || 'development',
        instanceIdentifier: generateUniqueIdentifier()
      },
      safeData
    );
    
    const response = await axios.post(`${process.env.LOGGING_URL}/api/log/LogInfo`, logInfoRequest);
  
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

function generateUniqueIdentifier() {
  return `instance-${Math.random().toString(36).substr(2, 9)}`;
}