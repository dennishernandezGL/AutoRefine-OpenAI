import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    // Avoid logging sensitive payment data
    const { /* Destructure or extract non-sensitive fields */ ...rest } = data;
    
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: '',
        environment: process.env.NODE_ENV || 'development',
        instanceIdentifier: process.env.INSTANCE_ID || 'unknown'
      },
      rest // Log only non-sensitive information
    );
    
    const logResponse = await axios.post('/api/log/LogInfo', logInfoRequest);
  
    return {
      statusCode: logResponse.status,
      data: logResponse.data,
    };
  } catch (error: any) {
    console.error('submitPayment error:', error);
    
    return {
      statusCode: error?.response?.status || 500,
      data: error?.response?.data || { error: 'Unknown error occurred' },
    };
  }  
};