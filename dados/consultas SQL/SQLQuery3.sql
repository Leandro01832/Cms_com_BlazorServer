select * from Content where Discriminator='UserContent'

update Content set Data=GETDATE() where Discriminator='UserContent'
