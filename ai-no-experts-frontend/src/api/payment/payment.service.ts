import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

const API_URL = process.env.API_URL || 'http://localhost:5050';  // Use environment variable

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: data?.user || 'unknown', // Populate loggerUser with available data
        environment: process.env.NODE_ENV || 'development', // Set environment
        instanceIdentifier: process.env.INSTANCE_ID || ''  // Set instance identifier if available
      },
      data
    );
    
    const response = await axios.post(`${API_URL}/api/log/LogInfo`, logInfoRequest);
  
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

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: '',
        environment: '',
        instanceIdentifier: ''
      },
      data
    );
    
    const response = await axios.post('http://localhost:5050/api/log/LogInfo', logInfoRequest);
  
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