import { Box, Button, Container, Grid, Typography } from '@mui/material';
import { Form, Formik } from 'formik';
import { type FunctionComponent } from 'react';
import * as Yup from 'yup';

import FormField from '../FormField/FormField';
import type { Payment } from '../../models/payment.model';

const PaymentForm: FunctionComponent<PaymentFormProps> = ({
  onSubmit,
}) => {

  /**
   * Formik Initial Setup
   */
  const initialValues = {
    fullName: '',
    email: '',
    cardNumber: '',
    expirationDate: '',
    cvv: '',
    billingAddress: '',
    billingAddress2: '',
    ssn: '',
    phone: '',
    country: '',
  };

  const validationSchema = Yup.object({
    fullName: Yup.string().required('Full Name is required'),
    email: Yup.string().email('Enter a valid email').required('Email is required'),
    cardNumber: Yup.string().matches(/^\d{16}$/, 'Card Number must be 16 digits').required('Card Number is required'),
    expirationDate: Yup.string().matches(/^(0[1-9]|1[0-2])\/\d{2}$/, 'Expiration date must be in MM/YY format').required('Expiration Date is required'),
    cvv: Yup.string().matches(/^\d{3,4}$/, 'CVV must be 3 or 4 digits').required('CVV is required'),
    billingAddress: Yup.string().required('Billing Address is required'),
    billingAddress2: Yup.string(),
    ssn: Yup.string().matches(/^\d{3}-?\d{2}-?\d{4}$/, 'SSN must be in valid format').notRequired(),
    phone: Yup.string().matches(/^\+?\d{10,15}$/, 'Phone number must be valid').notRequired(),
    country: Yup.string().required('Country is required'),
  });

  return (
    <Container>
      {/* Title */}
      <Box sx={{ marginBottom: '10px' }}>
        <Typography component={'h1'} sx={{ fontFamily: 'Agdasima', fontSize: '30px'}}>Payment Form:</Typography>
      </Box>
      
      <Formik
        initialValues={initialValues}
        validationSchema={validationSchema}
        onSubmit={(values: any) => {
          const { ssn, ...rest } = values;
          // Ensure ssn is handled securely (e.g., encryption before storage)
          onSubmit(rest);
        }}
      >
        {({ errors, touched, resetForm }) => (
          <Form>
            <Grid container spacing={2}>
              {/* Full Name */}
              <Grid item xs={12}>
                <FormField name='fullName' label='Full Name' error={touched.fullName && Boolean(errors.fullName)} />
              </Grid>

              {/* Email */}
              <Grid item xs={12}>
                <FormField name='email' label='Email' error={touched.email && Boolean(errors.email)} />
              </Grid>

              {/* Card Number */}
              <Grid item xs={12} md={6}>
                <FormField name='cardNumber' label='Credit Card Number' error={touched.cardNumber && Boolean(errors.cardNumber)} />
              </Grid>

              {/* Expiration Date */}
              <Grid item xs={12} md={4}>
                <FormField name='expirationDate' label='Expiration Date (MM/YY)' error={touched.expirationDate && Boolean(errors.expirationDate)} />
              </Grid>

              {/* CVV */}
              <Grid item xs={12} md={2}>
                <FormField name='cvv' label='CVV' error={touched.cvv && Boolean(errors.cvv)} />
              </Grid>

              {/* Billing Address */}
              <Grid item xs={12}>
                <FormField name='billingAddress' label='Billing Address' error={touched.billingAddress && Boolean(errors.billingAddress)} />
              </Grid>

              {/* Billing Address 2 */}
              <Grid item xs={12}>
                <FormField name='billingAddress2' label='Billing Address 2' error={touched.billingAddress2 && Boolean(errors.billingAddress2)} />
              </Grid>

              {/* SSN */}
              <Grid item xs={12} md={4}>
                <FormField name='ssn' label='Social Security Number' error={touched.ssn && Boolean(errors.ssn)} />
              </Grid>

              {/* Phone */}
              <Grid item xs={12} md={4}>
                <FormField name='phone' label='Phone' error={touched.phone && Boolean(errors.phone)} />
              </Grid>

              {/* Country */}
              <Grid item xs={12} md={4}>
                <FormField name='country' label='Country' error={touched.country && Boolean(errors.country)} />
              </Grid>
            </Grid>

            {/* Form Actions */}
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', marginTop: '20px' }}>
              <Button variant="text" color='primary' onClick={() => resetForm()}>Reset</Button>
              <Button type='submit' variant="contained" color='primary'>Submit</Button>
            </Box>
          </Form>
        )}        
      </Formik>
    </Container>
  );
}

interface PaymentFormProps {
  onSubmit: (values: Payment) => void;
}

export default PaymentForm;
