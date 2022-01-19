export class FormValidations {

  static getErrorMassage(fieldName: string, validatorName: string, validatorValue?: any): any {

    const config = {
      required: `${fieldName} é obrigatório!`,
      minlength: `${fieldName} precisa ter no mínimo ${validatorValue.requiredLength} caracteres.`,
      maxlength: `${fieldName} precisa ter no máximo ${validatorValue.requiredLength} caracteres.`,
      max: `${fieldName} não pode ser maior que ${validatorValue.max}.`,
      min: `${fieldName} não pode ser menor que ${validatorValue.min}.`,
      email: `Email inválido!`,
      mustMatch: `${fieldName}s não coincidem.`,
      pattern: `Aceite os ${fieldName}`
    };

    return config[validatorName];
  }
}
