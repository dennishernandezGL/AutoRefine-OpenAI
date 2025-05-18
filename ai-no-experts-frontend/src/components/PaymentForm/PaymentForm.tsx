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
    fullName: Yup.string().required('Full Name is required').matches(/^[a-zA-Z ]+$/, 'Full Name should only contain letters and spaces'),

    email: Yup.string().email('Enter a valid email').required('Email is required'),

    cardNumber: Yup.string().required('Card Number is required').matches(/^\d{16}$/, 'Card Number should be 16 digits'),

    expirationDate: Yup.string().required('Expiration Date is required').matches(/^(0[1-9]|1[0-2])\/\d{2}$/, 'Expiration Date should be in MM/YY format'),

    cvv: Yup.string().required('CVV is required').matches(/^\d{3,4}$/, 'CVV should be 3 or 4 digits'),

    billingAddress: Yup.string().required('Billing Address is required'),

    billingAddress2: Yup.string(),

    ssn: Yup.string().matches(/^\d{3}-\d{2}-\d{4}$/, 'SSN should be in the format XXX-XX-XXXX').optional(),

    phone: Yup.string().matches(/^\+?\d{10,15}$/, 'Phone Number should be 10 to 15 digits and can start with +').optional(),

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
        onSubmit={(values: any) => onSubmit(values)}
      >
        {({ errors, touched, resetForm }) => (
          <Form>
            <Grid container spacing={2}>
              {/* Full Name */}
              <Grid size={12}>
                <FormField name='fullName' label='Full Name' error={touched.fullName && Boolean(errors.fullName)} />
              </Grid>

              {/* Email */}
              <Grid size={12}>
                <FormField name='email' label='Email' error={touched.email && Boolean(errors.email)} />
              </Grid>

              {/* Card Number */}
              <Grid size={{ xs: 12, md: 6 }}>
                <FormField name='cardNumber' label='Credit Card Number' error={touched.cardNumber && Boolean(errors.cardNumber)} />
              </Grid>

              {/* Expiration Date */}
              <Grid size={{ xs: 12, md: 4 }}>
                <FormField name='expirationDate' label='Expiration Date (MM/YY)' error={touched.expirationDate && Boolean(errors.expirationDate)} />
              </Grid>

              {/* CVV */}
              <Grid size={{ xs: 12, md: 2 }}>
                <FormField name='cvv' label='CVV' error={touched.cvv && Boolean(errors.cvv)} />
              </Grid>

              {/* Billing Address */}
              <Grid size={12}>
                <FormField name='billingAddress' label='Billing Address' error={touched.billingAddress && Boolean(errors.billingAddress)} />
              </Grid>

              {/* Billing Address 2 */}
              <Grid size={12}>
                <FormField name='billingAddress2' label='Billing Address 2' error={touched.billingAddress2 && Boolean(errors.billingAddress2)} />
              </Grid>

              {/* SSN */}
              <Grid size={{ xs: 12, md: 4 }}>
                <FormField name='ssn' label='Social Security Number' error={touched.ssn && Boolean(errors.ssn)} />
              </Grid>

              {/* Phone */}
              <Grid size={{ xs: 12, md: 4 }}>
                <FormField name='phone' label='Phone' error={touched.phone && Boolean(errors.phone)} />
              </Grid>

              {/* Country */}
              <Grid size={{ xs: 12, md: 4 }}>
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
