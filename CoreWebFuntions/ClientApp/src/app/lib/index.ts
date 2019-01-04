function formatDate(date: Date): string {
  const d = new Date(date);
  let month = '' + (d.getMonth() + 1);
  let day = '' + d.getDate();
  const year = d.getFullYear();

  if (month.length < 2) {
    month = '0' + month;
  }
  if (day.length < 2) {
    day = '0' + day;
  }

  return [year, month, day].join('-');
}

export const buildQueryParams = (opt, prefix: string = ''): string =>
  opt == null
    ? ''
    : Object.keys(opt)
      .filter(key => {
        if (Array.isArray(opt[key] && opt[key].length === 0)) {
          return false;
        }
        return opt[key] != null;
      })
      .reduce((params, key) => {
        const value = opt[key];
        if (value.getMonth && typeof value.getMonth === 'function') {
          const dateString = formatDate(value);
          return params + `${prefix}${key}=${dateString}` + '&';
        }
        if (Array.isArray(value)) {
          return value.length === 0
            ? params
            : params +
            (<Array<any>>value)
              .map(k => `${prefix}${key}=${k}`)
              .join('&') +
            '&';
        }
        const isObject: boolean =
          typeof value === 'object' &&
          value instanceof Object &&
          !(value instanceof Array);
        if (isObject) {
          return params + buildQueryParams(value, key + '.');
        }

        return params + `${prefix}${key}=${opt[key]}` + '&';
      }, prefix === '' ? '?' : '');
