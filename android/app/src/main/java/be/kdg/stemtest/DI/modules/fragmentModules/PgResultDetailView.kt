package be.kdg.stemtest.DI.modules.fragmentModules

import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
abstract class PgResultDetailView {

    @ContributesAndroidInjector
    abstract fun bindPgResultDetailViewFragment(): PgResultDetailView
}