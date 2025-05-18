import { FormControl, InputAdornment, TextField } from '@mui/material';
import { Field, ErrorMessage } from 'formik';
import { type FunctionComponent } from 'react';
import React from 'react';

const FormField: FunctionComponent<FormFieldProps> = ({
    endAdornment,    
    fullWidth = true,
    label = '',
    name = '',
    startAdornment,
    type = 'text',
    error = false,
    ...props
}) => {
    return (
        <FormControl fullWidth margin="normal">
            <Field
                as={TextField}
                fullWidth
                id={name}
                name={name}
                label={label}
                type={type}
                error={error}
                InputProps={{
                    startAdornment: startAdornment ? <InputAdornment position="start">{React.isValidElement(startAdornment) ? startAdornment : React.createElement('span', {dangerouslySetInnerHTML: {__html: startAdornment}})}</InputAdornment> : null,
                    endAdornment: endAdornment ? <InputAdornment position="end">{React.isValidElement(endAdornment) ? endAdornment : React.createElement('span', {dangerouslySetInnerHTML: {__html: endAdornment}})}</InputAdornment> : null,
                }}
                {...props}
            />
            <ErrorMessage className='form-error' name={name} component="p" />
        </FormControl>
    );
}

type FormFieldProps = {
    endAdornment?: React.ReactNode;
    error?: boolean;
    fullWidth?: boolean,
    label: string;
    name: string;
    startAdornment?: React.ReactNode;
    type?: string;
    [key: string]: any;
}

export default FormField;