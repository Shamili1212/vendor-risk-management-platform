import type { Config } from 'tailwindcss';

export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'ui-sans-serif', 'system-ui', 'sans-serif']
      },
      colors: {
        ink: '#1f2937',
        surface: '#f7f8fb',
        marine: '#0f766e',
        saffron: '#b7791f',
        berry: '#be123c'
      }
    }
  },
  plugins: []
} satisfies Config;
