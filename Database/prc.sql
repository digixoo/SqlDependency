CREATE TABLE [dbo].[prc](
	[id_prc] [int] IDENTITY(1,1) NOT NULL,
	[prc_sec] [int] NOT NULL,
	[prc_nom_spr] [varchar](18) NULL,
	[prc_des_cor] [varchar](25) NULL,
	[prc_des] [varchar](250) NULL,
	[prc_est_eje] [char](1) NULL,
	[prc_est] [char](1) NULL,
	[prc_tip] [char](1) NULL,
	[prc_eje] [char](1) NULL,
	[prc_fec] [datetime] NULL,
	[prc_hra_ini] [datetime] NULL,
	[prc_hra_fin] [datetime] NULL,
	[prc_fec_rea] [datetime] NULL,
	[prc_par] [varchar](250) NULL
) ON [PRIMARY]
