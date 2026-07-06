import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { App } from './App';

describe('App', () => {
  it('renders dashboard KPIs and navigates to vendors', async () => {
    render(<App />);

    expect(screen.getByText('High-risk vendors')).toBeInTheDocument();

    await userEvent.click(screen.getByRole('button', { name: /vendors/i }));

    expect(screen.getByText('Northwind Cloud Services')).toBeInTheDocument();
  });
});
